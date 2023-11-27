using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Observability.CommonShared.DTOs;
using Observability.StockAPI.Services;

namespace Observability.StockAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly StockService _stockService;
        public StockController(StockService stockService)
        {
            _stockService = stockService;
        }

        [HttpPost]
        public async Task<IActionResult> CheckAndPaymentStart(StockCheckAndPaymentProcessRequestDto request)
        {
            var result = await _stockService.CheckAndPaymentProcess(request);

            return new ObjectResult(result)
            {
                StatusCode = result.StatusCode
            };
        }
    }
}
