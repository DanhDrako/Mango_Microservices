using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging.RabbitMQ.Base
{
    public abstract class RabbitMQBaseConsumer : BackgroundService // Mark the class as abstract
    {
        protected abstract string QueueName { get; }
        private IConnection _connection;
        private IChannel _channel;

        public RabbitMQBaseConsumer()
        {
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
                await _channel.QueueDeclareAsync(QueueName, false, false, false, null);
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
                //var message = JsonConvert.DeserializeObject<object>(body);
                HandleMessage(body);

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync(QueueName, false, consumer);
        }

        protected abstract void HandleMessage(string body); // No changes needed here
    }
}
