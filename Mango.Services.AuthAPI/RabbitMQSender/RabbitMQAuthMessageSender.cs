using log4net;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Services.AuthAPI.RabbitMQSender
{
    public class RabbitMQAuthMessageSender : IRabbitMQAuthMessageSender
    {
        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection;
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RabbitMQAuthMessageSender()
        {
            _hostName = "localhost";
            _password = "guest";
            _username = "guest";
        }

        public async Task SendMessage(object message, string queueName)
        {
            try
            {
                if (await ConnectionExists())
                {
                    using var channel = await _connection.CreateChannelAsync();
                    await channel.QueueDeclareAsync(queueName, false, false, false, null);
                    var json = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(json);
                    await channel.BasicPublishAsync(exchange: "", routingKey: queueName, body: body);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occur when send AuthAPI message", ex);
            }

        }

        private async Task CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    Password = _password,
                    UserName = _username
                };

                _connection = await factory.CreateConnectionAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<bool> ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }
            await CreateConnection();
            return true;
        }
    }
}
