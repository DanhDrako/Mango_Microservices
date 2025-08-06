using Mango.Services.PaymentAPI.Utility;

namespace Mango.Services.PaymentAPI.Models.Dto.Payment
{
    public class PaymentQueueDto
    {
        public required string PaymentIntentId { get; set; }
        public OrderStatus Status { get; set; }
        public double Total { get; set; }
    }
}
