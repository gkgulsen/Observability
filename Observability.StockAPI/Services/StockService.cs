using Observability.CommonShared.DTOs;

namespace Observability.StockAPI.Services
{
    public class StockService
    {
        private readonly PaymentService _paymentService;
        private readonly ILogger<StockService> _logger;

        public StockService(PaymentService paymentService, ILogger<StockService> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        public Dictionary<int, int> GertProductStockList()
        {
            return new Dictionary<int, int>
            {
                { 1, 10 },
                { 2, 20 },
                { 3, 30 },
                { 4, 40 },
                { 5, 50 },
            };
        }

        public async Task<ResponseDto<StockCheckAndPaymentProcessResponseDto>> CheckAndPaymentProcess(StockCheckAndPaymentProcessRequestDto request)
        {
            var productSTockList = GertProductStockList();

            var stockStatus = new List<(int productId, bool hasStockExist)>();

            foreach (var orderItem in request.OrderItems)
            {
                var hasExistStock = productSTockList.Any(x => x.Key == orderItem.ProductId && x.Value >= orderItem.Count);

                stockStatus.Add((orderItem.ProductId, hasExistStock));

            }
            if (stockStatus.Any(x => x.hasStockExist == false))
            {

                return ResponseDto<StockCheckAndPaymentProcessResponseDto>.Fail(StatusCodes.Status400BadRequest, "Stocks do not exist.");
            }

            //throw new Exception("Hata meydana geldi!!!");
            _logger.LogInformation("Stocks are left.orderCode: {@orderCode}", request.OrderCode);
            var (isSuccess, failMessage) = await _paymentService.CreatePaymentProcess(new()
            {
                OrderCode = request.OrderCode,
                TotalPrice = request.OrderItems.Sum(s => s.UnitPrice)
            });

            if (isSuccess)
            {
                return ResponseDto<StockCheckAndPaymentProcessResponseDto>.Success(
               StatusCodes.Status200OK,
               new StockCheckAndPaymentProcessResponseDto { Descripton = "Payment is successful." });
            }
            else
            {
                return ResponseDto<StockCheckAndPaymentProcessResponseDto>.Fail(StatusCodes.Status400BadRequest, failMessage!);
            }
        }
    }
}
