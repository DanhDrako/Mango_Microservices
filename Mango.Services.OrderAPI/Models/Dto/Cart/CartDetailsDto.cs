using Mango.Services.OrderAPI.Models.Dto.Product;

namespace Mango.Services.OrderAPI.Models.Dto.Cart
{
    public class CartDetailsDto
    {
        public int CartDetailsId { get; set; }
        public int Quantity { get; set; }

        public int ProductId { get; set; }
        public ProductDto? Product { get; set; }

        public int CartHeaderId { get; set; }
    }
}
