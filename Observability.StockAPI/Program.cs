using Observability.OpenTelemetryShared;
using Observability.CommonShared;
using Observability.StockAPI.Services;
using MassTransit;
using Observability.StockAPI.Consumers;
using Observability.Logging;
using Serilog;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

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
builder.Services.AddScoped<PaymentService>();
builder.Services.AddOpenTelemetryExt(builder.Configuration);

builder.Services.AddHttpClient<PaymentService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("PaymentService")["Url"]!);
});

builder.Services.AddScoped<StockService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"]!, "/", host =>
        {
            host.Username(builder.Configuration["RabbitMQ:Username"]!);
            host.Password(builder.Configuration["RabbitMQ:Password"]!);
        });

        cfg.ReceiveEndpoint("stock.order-created-event.queue", e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
        });

    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<OpenTelemetryTraceIdMiddleware>();
app.UseMiddleware<RequestAndResponseActivityMiddleware>();
app.UseExceptionMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
