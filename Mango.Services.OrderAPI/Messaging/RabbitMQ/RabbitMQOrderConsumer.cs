using Mango.Message.RabbitMQ.Consumer.Base;
using Mango.Services.OrderAPI.Models.Dto.Payment;
using Mango.Services.OrderAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.OrderAPI.Messaging.RabbitMQ
{
    public class RabbitMQOrderConsumer(
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory)
        : RabbitMQBaseConsumer
    {
        protected override string? ExchangeName => configuration["TopicAndQueueNames:PaymentCreatedTopic"] ??
            throw new ArgumentNullException("PaymentCreatedTopic");

        // Fix for CS0029: Convert the string to a KeyValuePair<string, string>
        protected override KeyValuePair<string, string> Queue => new(
            configuration["TopicAndQueueNames:PaymentCreatedSub_Order_Key"] ?? throw new ArgumentNullException("PaymentCreatedSub_Order_Key"),
            configuration["TopicAndQueueNames:PaymentCreatedSub_Order_Value"] ?? throw new ArgumentNullException("PaymentCreatedSub_Order_Value")
        );

        protected override async Task HandleMessageAsync(string body)
        {
            using var scope = scopeFactory.CreateScope();
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
            var message = JsonConvert.DeserializeObject<PaymentQueueDto>(body);
            if (message != null)
            {
                await orderService.UpdateOrderStatus(message);
            }
        }
    }
}
