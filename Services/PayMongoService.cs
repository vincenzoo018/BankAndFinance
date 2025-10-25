using System.Text;
using System.Text.Json;

namespace BankAndFinance.Services
{
    public interface IPayMongoService
    {
        Task<PayMongoResponse> CreatePaymentIntent(decimal amount, string description, string currency = "PHP");
        Task<PayMongoResponse> CreateSource(decimal amount, string type, string currency = "PHP");
        Task<PayMongoResponse> AttachToPaymentIntent(string paymentIntentId, string sourceId);
    }

    public class PayMongoService : IPayMongoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private readonly string _publicKey;
        private readonly string _baseUrl = "https://api.paymongo.com/v1";

        public PayMongoService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _secretKey = configuration["PayMongo:SecretKey"] ?? "sk_test_your_secret_key_here";
            _publicKey = configuration["PayMongo:PublicKey"] ?? "pk_test_your_public_key_here";
            
            // Set basic auth header
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_secretKey}:"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
        }

        public async Task<PayMongoResponse> CreatePaymentIntent(decimal amount, string description, string currency = "PHP")
        {
            try
            {
                var amountInCents = (int)(amount * 100);
                
                var payload = new
                {
                    data = new
                    {
                        attributes = new
                        {
                            amount = amountInCents,
                            payment_method_allowed = new[] { "card", "gcash", "grab_pay", "paymaya" },
                            payment_method_options = new
                            {
                                card = new { request_three_d_secure = "any" }
                            },
                            currency,
                            description,
                            statement_descriptor = "BFAS Payment"
                        }
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/payment_intents", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                return new PayMongoResponse
                {
                    Success = response.IsSuccessStatusCode,
                    Data = responseBody,
                    Message = response.IsSuccessStatusCode ? "Payment intent created" : "Failed to create payment intent"
                };
            }
            catch (Exception ex)
            {
                return new PayMongoResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<PayMongoResponse> CreateSource(decimal amount, string type, string currency = "PHP")
        {
            try
            {
                var amountInCents = (int)(amount * 100);
                
                var payload = new
                {
                    data = new
                    {
                        attributes = new
                        {
                            amount = amountInCents,
                            redirect = new
                            {
                                success = "https://yourdomain.com/payment/success",
                                failed = "https://yourdomain.com/payment/failed"
                            },
                            type,
                            currency
                        }
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/sources", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                return new PayMongoResponse
                {
                    Success = response.IsSuccessStatusCode,
                    Data = responseBody,
                    Message = response.IsSuccessStatusCode ? "Source created" : "Failed to create source"
                };
            }
            catch (Exception ex)
            {
                return new PayMongoResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<PayMongoResponse> AttachToPaymentIntent(string paymentIntentId, string sourceId)
        {
            try
            {
                var payload = new
                {
                    data = new
                    {
                        attributes = new
                        {
                            payment_method = sourceId
                        }
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/payment_intents/{paymentIntentId}/attach", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                return new PayMongoResponse
                {
                    Success = response.IsSuccessStatusCode,
                    Data = responseBody,
                    Message = response.IsSuccessStatusCode ? "Payment method attached" : "Failed to attach payment method"
                };
            }
            catch (Exception ex)
            {
                return new PayMongoResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
    }

    public class PayMongoResponse
    {
        public bool Success { get; set; }
        public string? Data { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
