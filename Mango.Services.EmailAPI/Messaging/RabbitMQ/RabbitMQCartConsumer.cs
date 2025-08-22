using Mango.Message.RabbitMQ.Consumer.Base;
using Mango.Services.EmailAPI.Models.Dto.Cart;
using Mango.Services.EmailAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.EmailAPI.Messaging.RabbitMQ
{
    public class RabbitMQCartConsumer(
        IConfiguration configuration,
        IEmailService emailService)
        : RabbitMQBaseConsumer
    {
        protected override string? QueueName => configuration["TopicAndQueueNames:EmailShoppingCartQueue"] ??
            throw new ArgumentNullException("EmailShoppingCartQueue");

        protected override async Task HandleMessageAsync(string body)
        {
            var message = JsonConvert.DeserializeObject<CartHeaderDto>(body);
            if (message != null)
            {
                await emailService.EmailCartAndLog(message);
            }
        }
    }
}
