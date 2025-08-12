
using Mango.Services.RewardAPI.Utility;

namespace Mango.Services.RewardAPI.Message
{
    public class PaymentQueueDto
    {
        public required string PaymentIntentId { get; set; }
        public OrderStatus Status { get; set; }
        public double Total { get; set; }
    }
}
