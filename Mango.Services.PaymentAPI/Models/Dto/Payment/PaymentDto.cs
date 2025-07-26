namespace Mango.Services.PaymentAPI.Models.Dto.Payment
{
    public class PaymentDto
    {
        public int OrderHeaderId { get; set; }
        public required long Total { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
    }
}
