using Mango.Message.RabbitMQ.Consumer.Base;
using Mango.Services.EmailAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.EmailAPI.Messaging.RabbitMQ
{
    public class RabbitMQAuthConsumer(
        IConfiguration configuration,
        IEmailService emailService)
        : RabbitMQBaseConsumer
    {
        protected override string? QueueName => configuration["TopicAndQueueNames:RegisterUserQueue"] ?? "";

        protected override async Task HandleMessageAsync(string body)
        {
            var message = JsonConvert.DeserializeObject<string>(body);
            if (message != null)
            {
                await emailService.EmailRegisterUserAndLog(message);
            }
        }
    }
}
