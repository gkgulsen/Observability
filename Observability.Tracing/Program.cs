using Observability.Tracing;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(OpenTelemetryConstant.ActivitySourceName)
    .ConfigureResource(config =>
    {
        config
        .AddService(OpenTelemetryConstant.ServiceName, serviceVersion: OpenTelemetryConstant.ServiceVersion)
        .AddAttributes(new List<KeyValuePair<string, object>>()
        {
            new KeyValuePair<string, object>("host.machineName",Environment.MachineName),
            new KeyValuePair<string, object>("host.environment","dev")
        });
    }).AddConsoleExporter().AddOtlpExporter().Build();


var serviceHelper = new ServiceHelper();

await serviceHelper.Work1();