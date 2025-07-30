using Mango.Services.PaymentAPI.Utility;

namespace Mango.Services.PaymentAPI.Models.Dto.Order
{
    public class OrderHeaderDto
    {
        public int OrderHeaderId { get; set; }
        public required string UserId { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double OrderTotal { get; set; }
        public double DeliveryFee { get; set; }
        public OrderStatus Status { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public PaymentSummary? PaymentSummary { get; set; }
        public ShippingAddress? ShippingAddress { get; set; }
        public IEnumerable<OrderDetailsDto> OrderDetails { get; set; } = [];
    }
}
