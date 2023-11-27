using Observability.CommonShared.DTOs;

namespace Observability.StockAPI.Services
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool isSuccess, string? failMessage)> CreatePaymentProcess(PaymentCreateRequestDto requestBody)
        {

            var response = await _httpClient.PostAsJsonAsync<PaymentCreateRequestDto>("/api/PaymentProcess/Create", requestBody);

            var responseContent = await response.Content.ReadFromJsonAsync<ResponseDto<PaymentCreateResponseDto>>();

            return response.IsSuccessStatusCode ? (true, null) : (false, responseContent?.Errors?.First());


        }
    }
}
