using MassTransit.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Observability.OpenTelemetryShared
{
    public static class OpenTelemetryExtentions
    {
        public static void AddOpenTelemetryExt(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OpenTelemetryConstant>(configuration.GetSection("OpenTelemetry"));
            var openTelemetryConstants = (configuration.GetSection("OpenTelemetry").Get<OpenTelemetryConstant>())!;

            ActivitySourceProvider.Source = new System.Diagnostics.ActivitySource(openTelemetryConstants.ActivitySourceName);

            services.AddOpenTelemetry().WithTracing(options =>
            {


                options.AddSource(openTelemetryConstants.ActivitySourceName)
                       .AddSource(DiagnosticHeaders.DefaultListenerName)//for MassTransit
                       .ConfigureResource(resource =>
                       {
                           resource.AddService(openTelemetryConstants.ServiceName, serviceVersion: openTelemetryConstants.ServiceVersion);
                       });

                options.AddAspNetCoreInstrumentation(opt =>
                {
                    opt.Filter = (context) => context.Request.Path.Value!.Contains("api", StringComparison.InvariantCulture);

                    //opt.RecordException = true;

                    //opt.EnrichWithException = (activity, exception) =>
                    //{
                    //    activity.SetTag("key1", exception.Message);
                    //};

                });

                options.AddEntityFrameworkCoreInstrumentation(opt =>
                {
                    opt.SetDbStatementForText = true;
                    opt.SetDbStatementForStoredProcedure = true;
                    opt.EnrichWithIDbCommand = (activity, dbCommand) =>
                    {

                    };
                });

                options.AddHttpClientInstrumentation(httpOptions =>
                {

                    httpOptions.FilterHttpRequestMessage = (request) =>
                    {
                        //return !request.RequestUri!.AbsolutePath.Contains("swagger", StringComparison.InvariantCulture);
                        return !request.RequestUri.AbsoluteUri.Contains("9200", StringComparison.InvariantCulture);
                    };




                    httpOptions.EnrichWithHttpRequestMessage = async (activity, request) =>
                    {
                        var requestContent = "empty";
                        if (request.Content != null)
                        {
                            requestContent = await request.Content.ReadAsStringAsync();
                        }

                        activity.SetTag("http.request.body", requestContent);
                    };


                    httpOptions.EnrichWithHttpResponseMessage = async (activity, response) =>
                    {
                        var responseContent = "empty";
                        if (response.Content != null)
                        {
                            responseContent = await response.Content.ReadAsStringAsync();
                        }

                        activity.SetTag("http.request.body", responseContent);
                    };
                });

                options.AddRedisInstrumentation(redisOpt =>
                {
                    //Ayrıca redis Instrumentation kullanacak API'de startup.cs içerisinde aşağıdaki kodu eklemek gerekiyor.
                    /*
                        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                        {
                            var redisService = sp.GetService<RedisService>();
                            return redisService!.GetConnectionMultixer;
                        });
                     
                     */
                    redisOpt.SetVerboseDatabaseStatements = true;
                });

                options.AddConsoleExporter();
                options.AddOtlpExporter(); //Jaeger
            });
        }
    }
}