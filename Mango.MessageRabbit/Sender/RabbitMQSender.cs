using log4net;
using Mango.Message.RabbitMQ.Sender.Interface;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Message.RabbitMQ.Sender
{
    public class RabbitMQSender : IRabbitMQSender
    {
        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection;
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RabbitMQSender()
        {
            _hostName = "localhost";
            _username = "guest";
            _password = "guest";
        }

        public async Task PublishMessage(object message, string queueName)
        {
            try
            {
                await ConnectionExists();

                using var channel = await _connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(queueName, false, false, false, null);
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);
                await channel.BasicPublishAsync(exchange: "", routingKey: queueName, body: body);
            }
            catch (Exception ex)
            {
                _logger.Error("[RabbitMQSender] Error occur when execute PublishMessage 2 param", ex);
            }
        }

        public async Task PublishMessage(object message, string exchangeName, Dictionary<string, string> queues)
        {
            try
            {
                await ConnectionExists();

                using var channel = await _connection.CreateChannelAsync();
                await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: false);
                foreach (KeyValuePair<string, string> queue in queues)
                {
                    await channel.QueueDeclareAsync(queue.Value, false, false, false, null);
                    await channel.QueueBindAsync(queue.Value, exchangeName, queue.Key);

                    var json = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(json);
                    await channel.BasicPublishAsync(exchange: exchangeName, queue.Key, body: body);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("[RabbitMQSender] Error occur when execute PublishMessage 3 param", ex);
            }
        }

        private async Task ConnectionExists()
        {
            if (_connection != null) return;
            await CreateConnection();
        }
        private async Task CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _username,
                    Password = _password
                };

                _connection = await factory.CreateConnectionAsync();
            }
            catch (Exception ex)
            {
                _logger.Error("[RabbitMQSender] Error occur when send execute CreateConnection", ex);
            }
        }
    }
}
