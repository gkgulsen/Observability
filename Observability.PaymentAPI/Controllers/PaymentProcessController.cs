using Microsoft.AspNetCore.Mvc;
using Observability.CommonShared.DTOs;
using System.Net;

namespace Observability.PaymentAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentProcessController : ControllerBase
    {
        private readonly ILogger<PaymentProcessController> _logger;

        public PaymentProcessController(ILogger<PaymentProcessController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Create(PaymentCreateRequestDto request)
        {
            const decimal balance = 1000;

            if (request.TotalPrice > balance)
            {
                _logger.LogWarning("Insufficient balance. {@OrderCode}", request.OrderCode);
                return BadRequest(ResponseDto<PaymentCreateResponseDto>.Fail(HttpStatusCode.BadRequest.GetHashCode(), "Insufficient balance"));
            }
               
            _logger.LogWarning("Payment is successful. {@OrderCode}", request.OrderCode);
            return Ok(ResponseDto<PaymentCreateResponseDto>.Success(HttpStatusCode.OK.GetHashCode(), new PaymentCreateResponseDto
            {
                Description = request.OrderCode
            }));
        }
    }
}
