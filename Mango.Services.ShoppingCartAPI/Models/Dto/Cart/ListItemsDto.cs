namespace Mango.Services.ShoppingCartAPI.Models.Dto.Cart
{
    public class ListItemsDto
    {
        public required string UserId { get; set; }
        public int[] Items { get; set; } = [];
    }
}
