using Mango.Services.OrderAPI.Utility;

namespace Mango.Services.OrderAPI.Models.Dto.Payment
{
    public class PaymentQueueDto
    {
        public required string PaymentIntentId { get; set; }
        public OrderStatus Status { get; set; }
        public double Total { get; set; }
    }
}
