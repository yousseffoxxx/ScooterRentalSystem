namespace ScooterRental.Service.PaymentServices
{
    public class PaymobService(IHttpClientFactory _httpClientFactory, IOptions<PaymobOptions> _options) : IPaymobService
    {

        public async Task<TopUpResponseDto> InitiateWalletPaymentAsync(decimal amount, string phoneNumber)
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

            var internalOrderId = Guid.NewGuid().ToString();

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
