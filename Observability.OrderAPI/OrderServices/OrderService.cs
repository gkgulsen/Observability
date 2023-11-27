using MassTransit;
using Observability.CommonShared.DTOs;
using Observability.CommonShared.Events;
using Observability.OpenTelemetryShared;
using Observability.OrderAPI.Models;
using Observability.OrderAPI.RedisServices;
using Observability.OrderAPI.StockServices;
using System.Diagnostics;
using System.Net;

namespace Observability.OrderAPI.OrderServices
{
    public class OrderService
    {
        private readonly AppDbContext _context;
        private readonly StockService _stockServices;
        private readonly RedisService _redisService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderService> _logger;

        public OrderService(AppDbContext context, StockService stockServices, RedisService redisService, IPublishEndpoint publishEndpoint, ILogger<OrderService> logger)
        {
            _context = context;
            _stockServices = stockServices;
            _redisService = redisService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<ResponseDto<OrderCreateResponseDto>> CreateAsync(OrderCreateRequestDto orderCreateRequestDto)
        {

            //using (var redisActivity=ActivitySourceProvider.Source.StartActivity("Redis"))
            //{
            //    await _redisService.GetDb(0).StringSetAsync("userId", orderCreateRequestDto.UserId);
            //}

            Activity.Current?.SetTag("Asp.Net Core(instrumentation) tag1", "Asp.Net Core(instrumentation) tag value");
            using var activity = ActivitySourceProvider.Source.StartActivity();
            activity?.AddEvent(new("Sipariş süreci başladı."));

            activity?.SetBaggage("userId", orderCreateRequestDto.UserId.ToString());

            var newOrder = new Order()
            {
                //Id = 2,
                Created = DateTime.Now,
                OrderCode = Guid.NewGuid().ToString(),
                Status = OrderStatus.Success,
                UserId = orderCreateRequestDto.UserId,
                Items = orderCreateRequestDto.Items.Select(x => new OrderItem()
                {
                    Count = x.Count,
                    ProductId = x.ProductId,
                    Price = x.UnitPrice
                }).ToList()
            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            _logger.LogInformation("Sipariş oluşturuldu. {@userId}", orderCreateRequestDto.UserId);
            //await _publishEndpoint.Publish(new OrderCreatedEvent() { OrderCode = newOrder.OrderCode });

            StockCheckAndPaymentProcessRequestDto processRequest = new()
            {
                OrderCode = newOrder.OrderCode,
                OrderItems = orderCreateRequestDto.Items
            };


            var (isSuccess, failMessage) = await _stockServices.CheckStockAndPaymentStartAsync(processRequest);

            if (!isSuccess)
            {
                return ResponseDto<OrderCreateResponseDto>.Fail(HttpStatusCode.InternalServerError.GetHashCode(), failMessage!);
            }

            activity?.AddEvent(new("Sipariş süreci tamamlandı..."));

            return ResponseDto<OrderCreateResponseDto>.Success(HttpStatusCode.OK.GetHashCode(), new OrderCreateResponseDto() { Id = newOrder.Id });


        }

    }
}
