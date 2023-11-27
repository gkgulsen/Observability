using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Observability.CommonShared.Events;
using Observability.OrderAPI.OrderServices;

namespace Observability.OrderAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        readonly OrderService _orderServices;
        private readonly IPublishEndpoint _publishEndpoint;
        public OrderController(OrderService orderServices, IPublishEndpoint publishEndpoint)
        {
            _orderServices = orderServices;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateRequestDto orderCreateRequestDto)
        {
            var result = await _orderServices.CreateAsync(orderCreateRequestDto);

            return new ObjectResult(result) { StatusCode = result.StatusCode };

            //Example for Exception
            //var a = 10;
            //var b = 0;
            //var c = a / b;

        }

        [HttpGet]
        public async Task<IActionResult> SendOrderCreatedEvent()
        {
            await _publishEndpoint.Publish(new OrderCreatedEvent() { OrderCode = Guid.NewGuid().ToString() });

            return Ok();
        }
    }
}
