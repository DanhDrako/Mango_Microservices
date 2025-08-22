using Mango.Message.RabbitMQ.Consumer.Base;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.RewardAPI.Messaging.RabbitMQ
{
    public class RabbitMQRewardConsumer(
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory)
        : RabbitMQBaseConsumer
    {
        protected override string? ExchangeName => configuration["TopicAndQueueNames:PaymentCreatedTopic"] ?? "";

        protected override KeyValuePair<string, string> Queue => new(
            configuration["TopicAndQueueNames:PaymentCreatedSub_Reward_Key"] ?? throw new ArgumentNullException("PaymentCreatedSub_Reward_Key"),
            configuration["TopicAndQueueNames:PaymentCreatedSub_Reward_Value"] ?? throw new ArgumentNullException("PaymentCreatedSub_Reward_Value")
        );

        protected override async Task HandleMessageAsync(string body)
        {
            using var scope = scopeFactory.CreateScope();
            var rewardService = scope.ServiceProvider.GetRequiredService<IRewardService>();
            var message = JsonConvert.DeserializeObject<PaymentQueueDto>(body);
            if (message != null)
            {
                await rewardService.UpdateRewards(message);
            }
        }
    }
}
