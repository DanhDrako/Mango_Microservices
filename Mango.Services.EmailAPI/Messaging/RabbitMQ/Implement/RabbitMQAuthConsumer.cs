using Mango.Services.EmailAPI.Messaging.RabbitMQ.Base;
using Mango.Services.EmailAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.EmailAPI.Messaging.RabbitMQ.Implement
{
    public class RabbitMQAuthConsumer(
        IConfiguration configuration,
        IEmailService emailService)
        : RabbitMQBaseConsumer
    {
        protected override string QueueName => configuration[("TopicAndQueueNames:RegisterUserQueue")] ?? "";

        protected override void HandleMessage(string body)
        {
            var message = JsonConvert.DeserializeObject<string>(body);
            emailService.EmailRegisterUserAndLog(message);
        }
    }
}
