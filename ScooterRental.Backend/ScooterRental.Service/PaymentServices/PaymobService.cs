namespace ScooterRental.Service.PaymentServices
{
    public class PaymobService(IHttpClientFactory _httpClientFactory, IOptions<PaymobOptions> _options, UserManager<User> _userManager,INotificationService _notificationService) : IPaymobService
    {
        public async Task<TopUpResponseDto> InitiateWalletPaymentAsync(decimal amount, string phoneNumber, string userId)
        {
            var httpClient = _httpClientFactory.CreateClient();

            // ---------------------------------------------------------
            // 1. Authentication
            // ---------------------------------------------------------

            var authPayload = new { api_key = _options.Value.ApiKey };

            var authJsonBody = new StringContent(
                JsonSerializer.Serialize(authPayload), Encoding.UTF8, "application/json");

            var authHttpResponse = await httpClient.PostAsync("https://accept.paymob.com/api/auth/tokens", authJsonBody);

            await EnsureSuccessOrThrowAsync(authHttpResponse, "Authentication (Step 1)");

            var tokenData = await authHttpResponse.Content.ReadFromJsonAsync<PaymobUnifiedResponse>();

            var token = tokenData?.Token ?? throw new InvalidOperationException("Failed to retrieve Paymob authentication token.");

            // ---------------------------------------------------------
            // 2. Order Registration
            // ---------------------------------------------------------

            var internalOrderId = userId;

            var orderPayload = new
            {
                auth_token = token,
                delivery_needed = "false",
                amount_cents = ((int)(amount * 100)).ToString(),
                currency = "EGP",
                merchant_order_id = internalOrderId
            };

            var orderJsonBody = new StringContent(
                JsonSerializer.Serialize(orderPayload), Encoding.UTF8, "application/json");

            var orderHttpResponse = await httpClient.PostAsync("https://accept.paymob.com/api/ecommerce/orders", orderJsonBody);
                        
            await EnsureSuccessOrThrowAsync(orderHttpResponse, "Order Registration (Step 2)");

            var orderData = await orderHttpResponse.Content.ReadFromJsonAsync<PaymobUnifiedResponse>();
           
            var paymobOrderId = orderData?.Id ?? throw new Exception("Order ID was null.");

            // ---------------------------------------------------------
            // 3. Payment Key Generation
            // ---------------------------------------------------------

            var paymentKeyPayload = new
            {
                auth_token = token,
                amount_cents = ((int)(amount * 100)).ToString(),
                order_id = paymobOrderId.ToString(),
                currency = "EGP",
                integration_id = int.Parse(_options.Value.WalletIntegrationId),
                billing_data = new
                {
                    apartment = "NA",
                    email = "dummy@email.com",
                    floor = "NA",
                    first_name = "NA",
                    street = "NA",
                    building = "NA",
                    phone_number = phoneNumber,
                    postal_code = "NA",
                    extra_description = "NA",
                    city = "NA",
                    country = "EG",
                    last_name = "NA",
                    state = "NA"
                }
            };

            var paymentKeyJsonBody = new StringContent(
                JsonSerializer.Serialize(paymentKeyPayload), Encoding.UTF8, "application/json");

            var paymentKeyPayloadHttpResponse = await httpClient.PostAsync("https://accept.paymob.com/api/acceptance/payment_keys", paymentKeyJsonBody);

            await EnsureSuccessOrThrowAsync(paymentKeyPayloadHttpResponse, "Payment Key Generation (Step 3)");

            var paymentKeyData = await paymentKeyPayloadHttpResponse.Content.ReadFromJsonAsync<PaymobUnifiedResponse>();

            var paymentKeyToken = paymentKeyData?.Token ?? throw new Exception("Payment Key was null.");

            // ---------------------------------------------------------
            // 4. Initiate Wallet Payment
            // ---------------------------------------------------------

            var walletPayload = new
            {
                source = new
                {
                    identifier = phoneNumber,
                    subtype = "WALLET"
                },
                payment_token = paymentKeyToken
            };

            var walletJsonBody = new StringContent(
                JsonSerializer.Serialize(walletPayload), Encoding.UTF8, "application/json");

            var walletHttpResponse = await httpClient.PostAsync("https://accept.paymob.com/api/acceptance/payments/pay", walletJsonBody);

            await EnsureSuccessOrThrowAsync(walletHttpResponse, "Initiate Wallet (Step 4)");

            var walletData = await walletHttpResponse.Content.ReadFromJsonAsync<PaymobUnifiedResponse>();

            return new TopUpResponseDto(walletData?.FinalUrl ?? "");
        }

        public async Task<bool> ProcessPaymobWebhook(string hmacFromRequest, string jsonBody)
        {
            using var document = JsonDocument.Parse(jsonBody);

            var root = document.RootElement;

            var obj = root.GetProperty("obj");

            string concatenatedString =
                obj.GetProperty("amount_cents").GetInt32().ToString() +
                obj.GetProperty("created_at").GetString() +
                obj.GetProperty("currency").GetString() +
                obj.GetProperty("error_occured").GetBoolean().ToString().ToLower() +
                obj.GetProperty("has_parent_transaction").GetBoolean().ToString().ToLower() +
                obj.GetProperty("id").GetInt32().ToString() +
                obj.GetProperty("integration_id").GetInt32().ToString() +
                obj.GetProperty("is_3d_secure").GetBoolean().ToString().ToLower() +
                obj.GetProperty("is_auth").GetBoolean().ToString().ToLower() +
                obj.GetProperty("is_capture").GetBoolean().ToString().ToLower() +
                obj.GetProperty("is_refunded").GetBoolean().ToString().ToLower() +
                obj.GetProperty("is_standalone_payment").GetBoolean().ToString().ToLower() +
                obj.GetProperty("is_voided").GetBoolean().ToString().ToLower() +
                obj.GetProperty("order").GetProperty("id").GetInt32().ToString() +
                obj.GetProperty("owner").GetInt32().ToString() +
                obj.GetProperty("pending").GetBoolean().ToString().ToLower() +
                obj.GetProperty("source_data").GetProperty("pan").GetString() +
                obj.GetProperty("source_data").GetProperty("sub_type").GetString() +
                obj.GetProperty("source_data").GetProperty("type").GetString() +
                obj.GetProperty("success").GetBoolean().ToString().ToLower();

            var computedHmac = CalculateHmacSha512(concatenatedString, _options.Value.HMAC);

            if (!string.Equals(computedHmac, hmacFromRequest, StringComparison.OrdinalIgnoreCase))
                throw new UnAuthorizedException("Invalid HMAC signature.");

            bool isSuccess = obj.GetProperty("success").GetBoolean();
            
            if (!isSuccess)
                return true;

            string merchantOrderId = obj.GetProperty("order").GetProperty("merchant_order_id").GetString()!;
            
            int amountCents = obj.GetProperty("amount_cents").GetInt32();

            if (!Guid.TryParse(merchantOrderId, out var parsedUserId))
                return true;

            var user = await _userManager.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == parsedUserId);

            if (user is null || user.Wallet is null)
                throw new UnAuthorizedException("User or Wallet Not Found");

            decimal amountEgp = amountCents / 100m;

            user.Wallet.Balance += amountEgp;
            user.Wallet.TotalToppedUp += amountEgp;
            user.Wallet.UpdatedAt = DateTimeOffset.UtcNow;

            await _userManager.UpdateAsync(user);

            if (!string.IsNullOrEmpty(user.FcmToken))
                await _notificationService.SendNotificationAsync(user.FcmToken,
                    "Payment Successful",
                    $"{amountEgp} EGP has been added to your wallet.",
                    new Dictionary<string, string> { { "action", "refresh_wallet" } });
            
            return true;
        }

        private string CalculateHmacSha512(string text, string key)
        {
            var encoding = new UTF8Encoding();
            var textBytes = encoding.GetBytes(text);
            var keyBytes = encoding.GetBytes(key);

            using var hash = new HMACSHA512(keyBytes);
            var hashBytes = hash.ComputeHash(textBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response, string stepName)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException($"Paymob {stepName} Failed. Status: {response.StatusCode}. Details: {errorContent}");
            }
        }
    }
}
