using Mango.Services.OrderAPI.Models.Dto.Product;

namespace Mango.Services.OrderAPI.Models.Dto.Order
{
    public class OrderDetailsDto
    {
        public int OrderDetailsId { get; set; }
        public int OrderHeaderId { get; set; }
        public int ProductId { get; set; }
        public ProductDto? Product { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
    }
}
