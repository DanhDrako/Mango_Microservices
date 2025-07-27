using Mango.Services.PaymentAPI.Models.Dto.Payment;
using Mango.Services.PaymentAPI.Models.Dto.Product;
using Mango.Services.PaymentAPI.Service.IService;
using Mango.Services.PaymentAPI.Utility;
using Stripe;

namespace Mango.Services.PaymentAPI.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly HttpContext _httpContext;

        public PaymentService(IConfiguration config, IOrderService orderService, IHttpContextAccessor httpContextAccessor, IProductService productService)
        {
            _config = config;
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
            // Use IHttpContextAccessor to access HttpContext
            _httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");
            _productService = productService;
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
                await service.UpdateAsync(paymentDto.PaymentIntentId, options);
            }
            return intent;
        }

        public async Task<PaymentDto> CreateOrUpdatePayment(PaymentDto paymentDto)
        {
            var intent = await CreateOrUpdatePaymentIntent(paymentDto) ?? throw new Exception("Failed to create or update payment intent.");

            var existingOrder = await _orderService.GetOrder(paymentDto.OrderHeaderId) ?? throw new Exception("Failied to get existingOrder");

            existingOrder.OrderTotal = paymentDto.Total;
            existingOrder.DeliveryFee = paymentDto.Total > 10000 ? 0 : 500;
            existingOrder.PaymentIntentId = intent.Id;
            existingOrder.ClientSecret = intent.ClientSecret;

            var result = await _orderService.UpdateHeaderDto(existingOrder) ?? throw new Exception("Failed to update order header with payment intent details.");

            paymentDto.OrderHeaderId = result.OrderHeaderId;
            paymentDto.PaymentIntentId = intent.Id;
            paymentDto.ClientSecret = intent.ClientSecret;
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
            // Get order by paymentIntentId
            var order = await _orderService.GetOrder(intent.Id) ?? throw new Exception("Order not found for payment intent");

            if (order.OrderTotal != intent.Amount)
            {
                order.Status = OrderStatus.PaymentMismatch;
            }
            else
            {
                order.Status = OrderStatus.PaymentReceived;
            }

            var result = await _orderService.UpdateHeaderDto(order);

            if (result == null) throw new Exception("Failed to update order status to PaymentReceived or PaymentMismatch.");
            //var basket = await context.Baskets.FirstOrDefaultAsync(x => x.PaymentIntentId == intent.Id);

            //if (basket != null) context.Baskets.Remove(basket);

            //await context.SaveChangesAsync();
        }

        private async Task HandlePaymentIntentFailed(PaymentIntent intent)
        {
            // Get order by paymentIntentId
            var order = await _orderService.GetOrder(intent.Id) ?? throw new Exception("Order not found for payment intent");

            // Get productList
            IEnumerable<ProductDto> productList = await _productService.GetProducts();
            foreach (var item in order.OrderDetails)
            {
                var productItem = productList.FirstOrDefault(x => x.ProductId == item.ProductId)
                    ?? throw new Exception("Product not found for order item");

                productItem.QuantityInStock += item.Quantity;
            }

            // TODO: need to update to database product

            order.Status = OrderStatus.PaymentFailed;

            // Update order status
            var result = await _orderService.UpdateHeaderDto(order);
            if (result == null) throw new Exception("Failed to update order status to PaymentFailed.");
        }
    }
}
