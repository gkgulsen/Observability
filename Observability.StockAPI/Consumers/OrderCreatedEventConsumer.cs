using Automatonymous;
using MassTransit;
using Observability.CommonShared.Events;
using System.Diagnostics;
using System.Text.Json;

namespace Observability.StockAPI.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        public Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {

            Thread.Sleep(2000);

            System.Diagnostics.Activity.Current?.SetTag("message.body", JsonSerializer.Serialize(context.Message));

            return Task.CompletedTask;
        }
    }
}
