using Mango.Services.PaymentAPI.Models.Dto.Payment;
using Mango.Services.PaymentAPI.Service.IService;
using Stripe;

namespace Mango.Services.PaymentAPI.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly IOrderService _orderService;

        public PaymentService(IConfiguration config, IOrderService orderService)
        {
            _config = config;
            _orderService = orderService;
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
    }
}
