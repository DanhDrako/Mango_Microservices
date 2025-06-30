namespace Mango.Services.ShoppingCartAPI.Models.Dto.Cart
{
    public class InputCartDto
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
