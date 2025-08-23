using log4net;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Message.RabbitMQ.Consumer.Base
{
    /// <summary>
    /// Abstract base class for RabbitMQ message consumers.
    /// Supports both simple queue-based consumption and exchange-based consumption with routing keys.
    /// Inherits from BackgroundService to run as a hosted service in ASP.NET Core applications.
    /// </summary>
    public abstract class RabbitMQBaseConsumer : BackgroundService
    {
        #region Connection Configuration Fields
        /// <summary>
        /// RabbitMQ server hostname (e.g., "localhost", "rabbit.company.com").
        /// </summary>
        private readonly string _hostName;

        /// <summary>
        /// RabbitMQ username for authentication. Default is "guest" for local development.
        /// </summary>
        private readonly string _username;

        /// <summary>
        /// RabbitMQ password for authentication. Should be secure in production environments.
        /// </summary>
        private readonly string _password;

        #endregion

        #region Abstract Configuration Properties
        /// <summary>
        /// Gets the queue name for simple queue-based consumption.
        /// Override this property to specify the target queue name.
        /// Used when consuming messages directly from a queue without exchanges.
        /// </summary>
        /// <remarks>
        /// When using simple queue consumption, only this property needs to be overridden.
        /// The queue will be declared with default settings (non-durable, non-exclusive, non-auto-delete).
        /// </remarks>
        protected virtual string? QueueName => null;

        /// <summary>
        /// Gets the exchange name for exchange-based consumption.
        /// Override this property along with <see cref="Queue"/> to specify the target exchange.
        /// Used when consuming messages through an exchange with routing keys.
        /// </summary>
        /// <remarks>
        /// When using exchange-based consumption, both this property and <see cref="Queue"/> must be overridden.
        /// The exchange will be declared as a direct exchange with non-durable settings.
        /// </remarks>
        protected virtual string? ExchangeName => null;

        /// <summary>
        /// Gets the queue configuration for exchange-based consumption.
        /// The Key represents the routing key, and the Value represents the queue name.
        /// Override this property along with <see cref="ExchangeName"/> for exchange-based consumption.
        /// </summary>
        /// <remarks>
        /// Example: new KeyValuePair&lt;string, string&gt;("order.created", "order-processing-queue")
        /// This creates a binding where messages with routing key "order.created" are routed to "order-processing-queue".
        /// </remarks>
        protected virtual KeyValuePair<string, string> Queue => default;

        /// <summary>
        /// Logger instance for this class using log4net for structured logging.
        /// </summary>
        /// <remarks>
        /// Static logger instance shared across all instances of this class for performance optimization.
        /// Configured to use the class type as logger name for better log categorization.
        /// </remarks>
        private static readonly ILog _logger = LogManager.GetLogger(typeof(RabbitMQBaseConsumer));
        #endregion

        #region RabbitMQ Infrastructure Fields

        /// <summary>
        /// RabbitMQ connection instance for this consumer.
        /// Manages the TCP connection to the RabbitMQ server and handles connection recovery.
        /// </summary>
        private IConnection? _connection;

        /// <summary>
        /// RabbitMQ channel instance for message consumption.
        /// Provides the communication channel for declaring queues, exchanges, and consuming messages.
        /// Each consumer should use its own dedicated channel for thread safety.
        /// </summary>
        private IChannel? _channel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMQBaseConsumer"/> class with default connection settings.
        /// Uses localhost with guest credentials, suitable for local development and testing.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when neither queue configuration is properly set in derived classes.
        /// </exception>
        /// <remarks>
        /// This constructor establishes the RabbitMQ connection synchronously during object construction
        /// to ensure the consumer is ready when the background service starts. In production, consider
        /// using the parameterized constructor with appropriate connection settings.
        /// </remarks>
        public RabbitMQBaseConsumer()
        {
            _hostName = "localhost";
            _username = "guest";
            _password = "guest";

            _logger.Info("[RabbitMQBaseConsumer] Initialized with default settings (localhost:guest)");

            // Initialize connection synchronously during construction
            // This ensures the consumer is ready when the background service starts
            Task.Run(() => CreateConnectionAsync()).Wait();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RabbitMQBaseConsumer"/> with custom connection settings.
        /// Recommended for production environments where specific connection parameters are required.
        /// </summary>
        /// <param name="hostName">RabbitMQ server hostname or IP address (e.g., "rabbit.company.com")</param>
        /// <param name="username">Username for authentication. Should have appropriate permissions for queue operations</param>
        /// <param name="password">Password for authentication. Should be stored securely (e.g., in configuration or secrets)</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null or empty</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when connection cannot be established or queue configuration is invalid
        /// </exception>
        /// <remarks>
        /// Connection credentials should be obtained from secure configuration sources in production.
        /// The connection is established synchronously during construction to validate configuration early.
        /// </remarks>
        public RabbitMQBaseConsumer(string hostName, string username, string password)
        {
            _hostName = hostName ?? throw new ArgumentNullException(nameof(hostName));
            _username = username ?? throw new ArgumentNullException(nameof(username));
            _password = password ?? throw new ArgumentNullException(nameof(password));

            _logger.Info($"[RabbitMQBaseConsumer] Initialized with custom settings ({hostName}:{username})");

            // Initialize connection synchronously during construction
            Task.Run(() => CreateConnectionAsync()).Wait();
        }

        #endregion

        #region Connection Management

        /// <summary>
        /// Creates and configures the RabbitMQ connection and channel infrastructure.
        /// Automatically determines the consumption pattern (simple queue vs. exchange-based) based on 
        /// overridden properties and sets up the appropriate RabbitMQ infrastructure.
        /// </summary>
        /// <returns>A task representing the asynchronous connection creation operation</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when configuration properties are invalid or incompatible
        /// </exception>
        /// <remarks>
        /// This method supports two consumption patterns:
        /// 1. Simple Queue: Override <see cref="QueueName"/> only
        /// 2. Exchange-based: Override both <see cref="ExchangeName"/> and <see cref="Queue"/>
        /// 
        /// All infrastructure (exchanges, queues, bindings) is created with non-durable settings
        /// suitable for development. For production, consider implementing durable configurations.
        /// </remarks>
        private async Task CreateConnectionAsync()
        {
            try
            {
                _logger.Info("[RabbitMQBaseConsumer] Initializing RabbitMQ connection...");

                #region Connection Factory Configuration

                // Create connection factory with provided credentials
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _username,
                    Password = _password
                };

                #endregion

                #region Connection and Channel Creation

                // Establish connection to RabbitMQ server
                _connection = await factory.CreateConnectionAsync();
                
                // Create a dedicated channel for this consumer
                _channel = await _connection.CreateChannelAsync();

                _logger.Info("[RabbitMQBaseConsumer] Connection and channel created successfully");

                #endregion

                #region Infrastructure Setup Based on Configuration

                // Determine consumption pattern and set up appropriate infrastructure
                if (!string.IsNullOrEmpty(ExchangeName) && !Queue.Equals(default(KeyValuePair<string, string>)))
                {
                    #region Exchange-Based Pattern Setup

                    _logger.Debug($"[RabbitMQBaseConsumer] Setting up exchange-based consumption - Exchange: {ExchangeName}, Queue: {Queue.Value}, Routing Key: {Queue.Key}");

                    // Declare the exchange
                    await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Direct, durable: false);

                    // Declare the queue
                    await _channel.QueueDeclareAsync(Queue.Value, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    // Bind queue to exchange with routing key
                    await _channel.QueueBindAsync(Queue.Value, ExchangeName, Queue.Key);

                    _logger.Info("[RabbitMQBaseConsumer] Exchange-based infrastructure setup completed");

                    #endregion
                }
                else if (!string.IsNullOrEmpty(QueueName))
                {
                    #region Simple Queue-Based Pattern Setup

                    _logger.Debug($"[RabbitMQBaseConsumer] Setting up simple queue consumption - Queue: {QueueName}");

                    // Declare the queue for direct consumption
                    await _channel.QueueDeclareAsync(QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    _logger.Info("[RabbitMQBaseConsumer] Simple queue infrastructure setup completed");

                    #endregion
                }
                else
                {
                    #region Configuration Validation

                    var errorMessage = "Invalid consumer configuration: Either QueueName must be provided for simple queue consumption, " +
                                     "or both ExchangeName and Queue must be provided for exchange-based consumption.";

                    _logger.Error($"[RabbitMQBaseConsumer] {errorMessage}");
                    throw new InvalidOperationException(errorMessage);

                    #endregion
                }

                #endregion

                _logger.Info("[RabbitMQBaseConsumer] RabbitMQ consumer initialization completed successfully");
            }
            catch (Exception ex)
            {
                // Enhanced error logging with full context for troubleshooting
                var errorMessage = $"Failed to create RabbitMQ connection and setup infrastructure: {ex.Message}";
                _logger.Error($"[RabbitMQBaseConsumer] {errorMessage}", ex);
                
                // Re-throw to allow calling code to handle the failure appropriately
                throw;
            }
        }

        #endregion

        #region BackgroundService Implementation

        /// <summary>
        /// Executes the background message consumption task as part of the hosted service lifecycle.
        /// Sets up the message consumer and begins listening for incoming messages from the configured queue.
        /// This method runs continuously until the service is stopped or cancellation is requested.
        /// </summary>
        /// <param name="stoppingToken">
        /// Cancellation token to monitor for stop requests from the host application
        /// </param>
        /// <returns>A task that represents the background execution operation</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the RabbitMQ channel is not properly initialized
        /// </exception>
        /// <remarks>
        /// This method implements the BackgroundService.ExecuteAsync override and handles:
        /// - Setting up the asynchronous message consumer
        /// - Processing incoming messages through the abstract <see cref="HandleMessageAsync"/> method
        /// - Manual message acknowledgment for reliable message processing
        /// - Graceful shutdown when cancellation is requested
        /// 
        /// Messages are processed with manual acknowledgment (autoAck: false) to ensure reliability.
        /// Failed message processing is logged but does not stop the consumer.
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            #region Pre-execution Validation

            // Check if cancellation was requested before starting
            stoppingToken.ThrowIfCancellationRequested();

            // Validate that the channel was successfully created during initialization
            if (_channel == null)
                throw new InvalidOperationException("RabbitMQ channel is not initialized. Connection setup may have failed.");

            #endregion

            #region Message Consumer Setup

            _logger.Debug("[RabbitMQBaseConsumer] Setting up message consumer...");

            // Create an asynchronous consumer for handling messages
            var consumer = new AsyncEventingBasicConsumer(_channel);

            // Configure the message received event handler with error handling
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                try
                {
                    #region Message Processing

                    // Convert message body from bytes to UTF-8 string
                    var body = Encoding.UTF8.GetString(ea.Body.ToArray());

                    _logger.Debug($"[RabbitMQBaseConsumer] Received message with delivery tag: {ea.DeliveryTag}");

                    // Process the message using the derived class implementation
                    await HandleMessageAsync(body);

                    // Acknowledge successful message processing to remove from queue
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

                    _logger.Debug($"[RabbitMQBaseConsumer] Successfully processed and acknowledged message: {ea.DeliveryTag}");

                    #endregion
                }
                catch (Exception ex)
                {
                    #region Message Processing Error Handling

                    _logger.Error($"[RabbitMQBaseConsumer] Error processing message {ea.DeliveryTag}: {ex.Message}", ex);

                    // Note: In production environments, consider implementing:
                    // - Dead letter queues for failed messages
                    // - Retry mechanisms with exponential backoff
                    // - Message rejection with requeue strategies
                    // Currently, failed messages remain unacknowledged

                    #endregion
                }
            };

            #endregion

            #region Message Consumption Activation

            // Determine which queue to consume from based on configuration pattern
            var queueToConsume = !string.IsNullOrEmpty(QueueName) ? QueueName : Queue.Value;

            _logger.Info($"[RabbitMQBaseConsumer] Starting message consumption from queue: {queueToConsume}");

            // Start consuming messages with manual acknowledgment for reliability
            // autoAck = false ensures messages aren't removed until explicitly acknowledged
            await _channel.BasicConsumeAsync(queueToConsume, autoAck: false, consumer: consumer);

            _logger.Info("[RabbitMQBaseConsumer] Message consumption started successfully");

            #endregion

            #region Service Lifecycle Management

            // Keep the service running until cancellation is requested
            // This allows the message consumer to process messages continuously
            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.Info("[RabbitMQBaseConsumer] Consumption cancelled - shutting down gracefully");
            }

            #endregion
        }

        #endregion

        #region Abstract Contract

        /// <summary>
        /// Handles incoming messages from RabbitMQ queue or exchange.
        /// This method must be implemented by derived classes to define specific message processing logic
        /// appropriate for their business domain (e.g., order processing, email sending, reward calculation).
        /// </summary>
        /// <param name="body">
        /// The message body as a UTF-8 encoded string. The content format depends on the message producer
        /// and may be JSON, XML, plain text, or other formats as defined by your messaging contract.
        /// </param>
        /// <returns>A task representing the asynchronous message handling operation</returns>
        /// <remarks>
        /// <para>
        /// This method is called for each message received from the queue or exchange.
        /// Implementation guidelines:
        /// </para>
        /// <list type="bullet">
        /// <item><description>Parse the message body according to your expected format</description></item>
        /// <item><description>Perform business logic processing</description></item>
        /// <item><description>Handle errors gracefully - exceptions will be logged but won't stop the consumer</description></item>
        /// <item><description>Avoid long-running operations that could block other messages</description></item>
        /// <item><description>Consider implementing idempotency for reliable processing</description></item>
        /// </list>
        /// <para>
        /// Message acknowledgment is handled automatically by the base class upon successful completion.
        /// If this method throws an exception, the message will not be acknowledged and may be redelivered.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// protected override async Task HandleMessageAsync(string body)
        /// {
        ///     var orderMessage = JsonSerializer.Deserialize&lt;OrderCreatedMessage&gt;(body);
        ///     await _orderService.ProcessOrderAsync(orderMessage.OrderId);
        ///     _logger.Info($"Processed order: {orderMessage.OrderId}");
        /// }
        /// </code>
        /// </example>
        protected abstract Task HandleMessageAsync(string body);

        #endregion

        #region Resource Management

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// Properly disposes of RabbitMQ connections and channels to prevent resource leaks.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method ensures proper cleanup of RabbitMQ resources in the following order:
        /// </para>
        /// <list type="number">
        /// <item><description>Dispose the channel (closes message consumption)</description></item>
        /// <item><description>Dispose the connection (closes TCP connection to RabbitMQ)</description></item>
        /// <item><description>Call base.Dispose() to clean up BackgroundService resources</description></item>
        /// </list>
        /// <para>
        /// The disposal process is protected against exceptions to ensure that all cleanup steps
        /// are attempted even if individual steps fail. Any errors during disposal are logged
        /// but do not prevent the cleanup process from completing.
        /// </para>
        /// </remarks>
        public override void Dispose()
        {
            try
            {
                _logger.Debug("[RabbitMQBaseConsumer] Disposing RabbitMQ resources...");

                // Dispose channel first to stop message consumption
                _channel?.Dispose();

                // Then dispose connection to close TCP connection
                _connection?.Dispose();

                _logger.Info("[RabbitMQBaseConsumer] RabbitMQ resources disposed successfully");
            }
            catch (Exception ex)
            {
                _logger.Error("[RabbitMQBaseConsumer] Error during disposal", ex);
            }
            finally
            {
                // Always call base dispose to ensure proper cleanup of BackgroundService
                // This ensures the hosted service is properly removed from the service collection
                base.Dispose();
            }
        }

        #endregion
    }
}
