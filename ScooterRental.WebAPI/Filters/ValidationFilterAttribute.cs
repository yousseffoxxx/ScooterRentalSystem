namespace ScooterRental.WebAPI.Filters
{
    public class ValidationFilterAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .ToDictionary(
                        k => k.Key,
                        v => v.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                var errorResponse = new
                {
                    Success = false,
                    Message = "One or more validation errors occurred.",
                    Errors = errors
                };
                context.Result = new BadRequestObjectResult(errorResponse);
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
