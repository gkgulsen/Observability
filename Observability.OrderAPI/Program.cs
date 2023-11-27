using Observability.OrderAPI.OrderServices;
using Observability.OpenTelemetryShared;
using Observability.CommonShared;
using Observability.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;
using Observability.OrderAPI.StockServices;
using Observability.OrderAPI.RedisServices;
using StackExchange.Redis;
using MassTransit;
using Serilog;
using Observability.Logging;
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
builder.Services.AddScoped<OrderService>();
//builder.Services.AddScoped<StockService>();
builder.Services.AddOpenTelemetryExt(builder.Configuration);
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisService = sp.GetService<RedisService>();
    return redisService!.GetConnectionMultixer;
});
builder.Services.AddSingleton(_ =>
{
    return new RedisService(builder.Configuration);
});

builder.Services.AddHttpClient<StockService>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration["StockService:Url"]!);
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"]!, "/", host =>
        {
            host.Username(builder.Configuration["RabbitMQ:Username"]!);
            host.Password(builder.Configuration["RabbitMQ:Password"]!);
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
