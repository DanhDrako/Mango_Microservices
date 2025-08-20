using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.RewardAPI.Messaging.RabbitMQ.Base
{
    public abstract class RabbitMQBaseConsumer : BackgroundService // Mark the class as abstract
    {
        protected abstract string ExchangeName { get; }
        protected abstract KeyValuePair<string, string> Queue { get; }

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
                await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Direct);

                await _channel.QueueDeclareAsync(Queue.Value, false, false, false, null);
                await _channel.QueueBindAsync(Queue.Value, ExchangeName, Queue.Key);
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
                await HandleMessageAsync(body);

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync(Queue.Value, false, consumer);
        }

        protected abstract Task HandleMessageAsync(string body);
    }
}
