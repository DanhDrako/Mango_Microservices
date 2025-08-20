using Mango.Services.EmailAPI.Messaging.RabbitMQ.Base;
using Mango.Services.EmailAPI.Models.Dto.Cart;
using Mango.Services.EmailAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.EmailAPI.Messaging.RabbitMQ.Implement
{
    public class RabbitMQCartConsumer(
        IConfiguration configuration,
        IEmailService emailService)
        : RabbitMQBaseConsumer
    {
        protected override string QueueName => configuration[("TopicAndQueueNames:EmailShoppingCartQueue")] ??
            throw new ArgumentNullException("EmailShoppingCartQueue");

        protected override void HandleMessage(string body)
        {
            var message = JsonConvert.DeserializeObject<CartHeaderDto>(body);
            emailService.EmailCartAndLog(message);
        }
    }
}
