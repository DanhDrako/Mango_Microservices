using Mango.Services.EmailAPI.Service.IService;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class RabbitMQAuthConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private IConnection _connection;
        private IChannel _channel;
        public RabbitMQAuthConsumer(IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            Task.Run(() => CreateConnection()).Wait(); // Ensure the connect
        }

        private async Task CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest"
                };
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await _channel.QueueDeclareAsync(_configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"), false, false, false, null);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error creating RabbitMQ connection: {ex.Message}");
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonConvert.DeserializeObject<string>(body);
                await HandleMessage(message);

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync(_configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"), false, consumer);
        }

        private async Task HandleMessage(string email)
        {
            await _emailService.EmailRegisterUserAndLog(email);
        }
    }
}
