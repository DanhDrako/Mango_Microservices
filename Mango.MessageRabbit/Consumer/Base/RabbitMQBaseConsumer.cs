using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Message.RabbitMQ.Consumer.Base
{
    public abstract class RabbitMQBaseConsumer : BackgroundService
    {
        // Properties for simple queue-based consumption
        protected virtual string? QueueName => null;
        
        // Properties for exchange-based consumption
        protected virtual string? ExchangeName => null;
        protected virtual KeyValuePair<string, string> Queue => default;

        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMQBaseConsumer()
        {
            Task.Run(() => CreateConnection()).Wait();
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

                // Support both patterns based on which properties are overridden
                if (!string.IsNullOrEmpty(ExchangeName) && !Queue.Equals(default(KeyValuePair<string, string>)))
                {
                    // Exchange-based pattern
                    await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Direct);
                    await _channel.QueueDeclareAsync(Queue.Value, false, false, false, null);
                    await _channel.QueueBindAsync(Queue.Value, ExchangeName, Queue.Key);
                }
                else if (!string.IsNullOrEmpty(QueueName))
                {
                    // Simple queue-based pattern
                    await _channel.QueueDeclareAsync(QueueName, false, false, false, null);
                }
                else
                {
                    throw new InvalidOperationException("Either QueueName or both ExchangeName and Queue must be provided.");
                }
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

            if (_channel == null)
                throw new InvalidOperationException("Channel is not initialized.");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                await HandleMessageAsync(body);
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            // Consume from the appropriate queue
            var queueToConsume = !string.IsNullOrEmpty(QueueName) ? QueueName : Queue.Value;
            await _channel.BasicConsumeAsync(queueToConsume, false, consumer);
        }

        protected abstract Task HandleMessageAsync(string body);

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
