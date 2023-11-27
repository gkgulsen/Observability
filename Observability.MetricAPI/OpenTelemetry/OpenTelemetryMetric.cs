using System.Diagnostics.Metrics;

namespace Observability.MetricAPI.OpenTelemetry
{
    public static class OpenTelemetryMetric
    {
        private static readonly Meter meter = new Meter("metric.meter.api");

        public static Counter<int> OrderCreatedEventCounter=meter.CreateCounter<int>("order.created.event.count");
    }
}
