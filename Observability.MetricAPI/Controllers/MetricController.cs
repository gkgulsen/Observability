using Microsoft.AspNetCore.Mvc;
using Observability.MetricAPI.OpenTelemetry;

namespace Observability.MetricAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MetricController : ControllerBase
    {

        [HttpGet]
        public IActionResult CounterMetric()
        {
            OpenTelemetryMetric.OrderCreatedEventCounter.Add(1,
                new KeyValuePair<string, object?>("event", "add"),
                new KeyValuePair<string, object?>("queue.name", "event.created.queue")
                );

            return Ok();
        }

    }
}