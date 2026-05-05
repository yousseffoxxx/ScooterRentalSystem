namespace ScooterRental.Service.EmailServices
{
    public class EmailService(IConfiguration _configuration, ILogger<EmailService> _logger) : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var apiKey = _configuration.GetSection("EmailSettings")["SendGridApiKey"];
            var fromEmail = _configuration.GetSection("EmailSettings")["FromEmail"];
            var fromName = _configuration.GetSection("EmailSettings")["FromName"];

            var client = new SendGridClient(apiKey);
            var fromAddress = new EmailAddress(fromEmail);
            var toAddress = new EmailAddress(to);

            var msg = MailHelper.CreateSingleEmail(fromAddress, toAddress, subject: subject, plainTextContent: "", htmlContent: body);

            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Email successfully sent to {ToEmail}", to);
            else
                _logger.LogError("Failed to send email to {ToEmail}. SendGrid Status: {StatusCode}", to, response.StatusCode);
        }
        public async Task SendPasswordResetEmailAsync(string to, string resetLink)
        {
            var subject = "Scooter Rental - Password Reset";

            var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2>Reset Your Password</h2>
                    <p>We received a request to reset the password for your Scooter Rental account.</p>
                    <p>Click the button below to set a new password:</p>
                    <a href='{resetLink}' style='display: inline-block; padding: 10px 20px; color: white; background-color: #007bff; text-decoration: none; border-radius: 5px;'>Reset Password</a>
                    <p style='margin-top: 20px; font-size: 12px; color: gray;'>If you didn't request this, you can safely ignore this email.</p>
                </div>";

            await SendEmailAsync(to, subject, htmlBody);
        }
    }
}
