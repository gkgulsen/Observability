using Observability.CommonShared;
using Observability.Logging;
using Observability.OpenTelemetryShared;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
//builder.Host.UseSerilog(Logging.ConfigureLogging);
//builder.AddOpenTelemetryLog();
builder.Logging.AddOpenTelemetry(cfg =>
{
    var serviceName = builder.Configuration["OpenTelemetry:ServiceName"];
    var serviceVersion = builder.Configuration["OpenTelemetry:ServiceVersion"];

    cfg.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion));
    cfg.AddOtlpExporter((x, y) => { });
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenTelemetryExt(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<OpenTelemetryTraceIdMiddleware>();
app.UseMiddleware<RequestAndResponseActivityMiddleware>();
app.UseExceptionMiddleware();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
