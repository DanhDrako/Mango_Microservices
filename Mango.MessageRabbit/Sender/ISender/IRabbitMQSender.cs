namespace Mango.Message.RabbitMQ.Sender.Interface
{
    public interface IRabbitMQSender
    {
        Task PublishMessage(object message, string queueName);
        Task PublishMessage(object message, string exchangeName, Dictionary<string, string> queues);
    }
}
