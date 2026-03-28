namespace ScooterRental.WebAPI.CustomMiddleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate _next, ILogger<ExceptionHandlingMiddleware> _logger)
    {
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);

                await HandleUnmappedEndpointsAsync(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex,_logger);
            }
        }

        public static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception, ILogger logger)
        {

            if (exception is not AppException and not AppValidationException)
                logger.LogError(exception, "An unexpected server error occurred while accessing {Path}", httpContext.Request.Path);
            else
                logger.LogWarning("AppException thrown: {Message}", exception.Message);

            var response = new ReturnedError
            {
                ErrorMessage = exception.Message
            };

            if (!httpContext.Response.HasStarted)
            {
                httpContext.Response.StatusCode = exception switch
                {
                    AppValidationException validationException => HandleValidationException(validationException, response),
                    AppException appException => (int)appException.StatusCode,
                    _ => StatusCodes.Status500InternalServerError
                };

                response.StatusCode = httpContext.Response.StatusCode;

                await httpContext.Response.WriteAsJsonAsync(response);
            }
        }

        private async Task HandleUnmappedEndpointsAsync(HttpContext httpContext)
        {
            if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound && !httpContext.Response.HasStarted)
            {
                var response = new ReturnedError()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    ErrorMessage = $"End Point {httpContext.Request.Path} Is Not Found"
                };

                await httpContext.Response.WriteAsJsonAsync(response);
            }
        }
        
        private static int HandleValidationException(AppValidationException validationException, ReturnedError response)
        {
            response.Errors = validationException.Errors;

            return StatusCodes.Status400BadRequest;
        }
    }
}
