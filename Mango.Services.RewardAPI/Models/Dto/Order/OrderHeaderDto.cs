﻿using Mango.Services.RewardAPI.Utility;

namespace Mango.Services.RewardAPI.Models.Dto.Order
{
    public class OrderHeaderDto
    {
        public int OrderHeaderId { get; set; }
        public required string UserId { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double OrderTotal { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public double DeliveryFee { get; set; }
        public DateTime OrderTime { get; set; }
        public OrderStatus Status { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public PaymentSummary? PaymentSummary { get; set; }
        public ShippingAddress? ShippingAddress { get; set; }
        public IEnumerable<OrderDetailsDto> OrderDetails { get; set; } = [];
    }
}
