using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.Logging
{
    public static class Logging
    {
        public static void AddOpenTelemetryLog(this WebApplicationBuilder builder)
        {
            builder.Logging.AddOpenTelemetry(cfg =>
            {
                var serviceName = builder.Configuration["OpenTelemetry:ServiceName"];
                var serviceVersion = builder.Configuration["OpenTelemetry:ServiceVersion"];

                cfg.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion));
                cfg.AddOtlpExporter();
            });
        }
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogging => (builderContext, loggerConfiguration) =>
        {
            var environment = builderContext.HostingEnvironment;

            loggerConfiguration
                .ReadFrom.Configuration(builderContext.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Environment", environment.EnvironmentName)
                .Enrich.WithProperty("ApplicationName", environment.ApplicationName);

            var elasticSearchBaseUrl = builderContext.Configuration["ElasticSearch:BaseUrl"];
            var userName = builderContext.Configuration["ElasticSearch:UserName"];
            var password = builderContext.Configuration["ElasticSearch:Password"];
            var indexName = builderContext.Configuration["ElasticSearch:IndexName"];

            loggerConfiguration
                .WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(elasticSearchBaseUrl!))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = Serilog.Sinks.Elasticsearch.AutoRegisterTemplateVersion.ESv8,
                    IndexFormat = $"{indexName}-{environment.EnvironmentName}-logs-" + "{0:yyy.MM.dd}",
                    ModifyConnectionSettings = x => x.BasicAuthentication(userName, password),
                    CustomFormatter = new Serilog.Formatting.Elasticsearch.ElasticsearchJsonFormatter()

                });
        };
    }
}
