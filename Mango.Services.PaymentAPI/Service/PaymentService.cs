using Mango.MessageBus;
using Mango.Services.PaymentAPI.Models.Dto.Payment;
using Mango.Services.PaymentAPI.Service.IService;
using Mango.Services.PaymentAPI.Utility;
using Stripe;

namespace Mango.Services.PaymentAPI.Service
{
    public class PaymentService : IPaymentService
    {
        private const string PaymentCreatedTopicName = "TopicAndQueueNames:PaymentCreatedTopic";
        private readonly IConfiguration _config;
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpContext _httpContext;
        private readonly IMessageBus _messageBus;

        public PaymentService(IConfiguration config, IOrderService orderService,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
            IMessageBus messageBus)
        {
            _config = config;
            _orderService = orderService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            // Use IHttpContextAccessor to access HttpContext
            _httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");
            _messageBus = messageBus;
        }

        public async Task<PaymentIntent> CreateOrUpdatePaymentIntent(PaymentDto paymentDto)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            var service = new PaymentIntentService();

            var intent = new PaymentIntent();
            var deliveryFee = paymentDto.Total > 10000 ? 0 : 500;

            if (string.IsNullOrEmpty(paymentDto.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = paymentDto.Total + deliveryFee,
                    Currency = "usd",
                    PaymentMethodTypes = ["card"]
                };
                intent = await service.CreateAsync(options);
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = paymentDto.Total + deliveryFee
                };
                intent = await service.UpdateAsync(paymentDto.PaymentIntentId, options);
            }
            return intent;
        }

        public async Task<PaymentDto> CreateOrUpdatePayment(PaymentDto paymentDto)
        {
            var intent = await CreateOrUpdatePaymentIntent(paymentDto) ?? throw new Exception("Failed to create or update payment intent.");

            var existingOrder = await _orderService.GetOrder(paymentDto.OrderHeaderId) ?? throw new Exception("Failied to get existingOrder");

            // set data updating for OrderAPI
            existingOrder.OrderTotal = paymentDto.Total;
            existingOrder.DeliveryFee = paymentDto.Total > 10000 ? 0 : 500;
            existingOrder.PaymentIntentId = intent.Id;
            existingOrder.ClientSecret = intent.ClientSecret;

            var result = await _orderService.UpdateHeaderDto(existingOrder)
                ?? throw new Exception("Failed to update order header with payment intent details.");

            // set data return for current API
            paymentDto.OrderHeaderId = result.OrderHeaderId;
            paymentDto.PaymentIntentId = intent.Id;
            paymentDto.ClientSecret = intent.ClientSecret;
            paymentDto.UserId = result.UserId;

            return paymentDto;
        }

        public async Task<bool> ProcessPayment(string json)
        {
            var stripeEvent = ConstructStripeEvent(json);

            if (stripeEvent.Data.Object is not PaymentIntent intent) throw new Exception("Invalid event data");

            if (intent.Status == "succeeded") await HandlePaymentIntentSucceeded(intent);
            else await HandlePaymentIntentFailed(intent);

            return true;
        }

        private Event ConstructStripeEvent(string json)
        {
            try
            {
                return EventUtility.ConstructEvent(json, _httpContext.Request.Headers["Stripe-Signature"], _config["StripeSettings:WhSecret"]);
            }
            catch (Exception ex)
            {
                throw new StripeException("Invalid signature", ex);
            }
        }

        private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
        {
            // need sent event to update orderStatus, clear order details, and update product stock
            string topicName = _configuration.GetValue<string>(PaymentCreatedTopicName) ??
                throw new Exception("Cannot get PaymentTopicName");

            PaymentQueueDto paymentQueueDto = new()
            {
                PaymentIntentId = intent.Id,
                Status = OrderStatus.PaymentReceived,
                Total = intent.Amount
            };

            await _messageBus.PublishMessage(paymentQueueDto, topicName);
        }

        private async Task HandlePaymentIntentFailed(PaymentIntent intent)
        {
            // need sent event to update orderStatus, and update product stock
            string topicName = _configuration.GetValue<string>(PaymentCreatedTopicName) ??
                throw new Exception("Cannot get PaymentTopicName");

            PaymentQueueDto paymentQueueDto = new()
            {
                PaymentIntentId = intent.Id,
                Status = OrderStatus.PaymentFailed,
                Total = intent.Amount
            };

            await _messageBus.PublishMessage(paymentQueueDto, topicName);
        }
    }
}
