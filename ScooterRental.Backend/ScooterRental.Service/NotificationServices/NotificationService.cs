namespace ScooterRental.Service.NotificationServices
{
    public class NotificationService(ILogger<NotificationService> _logger, IAuthService _authService) : INotificationService
    {
        public async Task<string> SendNotificationAsync(string token, string title, string body, IDictionary<string, string>? hiddenDataPayload = null)
        {
            try
            {
                var message = new Message()
                {
                    Token = token,
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body
                    },
                    Data = hiddenDataPayload != null && hiddenDataPayload.Any() ? new Dictionary<string, string>(hiddenDataPayload): null
                };

                var messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message);

                return messageId;
            }
            catch (FirebaseMessagingException ex)
            {
                if (ex.MessagingErrorCode == MessagingErrorCode.Unregistered)
                {
                    _logger.LogWarning("FCM Token is unregistered or dead. Token: {Token}", token);

                    await _authService.RemoveDeadFcmTokenAsync(token);
                }
                else
                {
                    _logger.LogError(ex, "Firebase messaging error occurred while sending to {Token}", token);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while sending FCM notification to {Token}", token);

                return string.Empty;
            }
        }
    }
}
