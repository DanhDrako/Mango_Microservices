namespace Mango.Services.AuthAPI.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public string? UserId { get; set; }
    }
}
