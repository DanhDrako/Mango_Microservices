namespace Mango.Services.OrderAPI.Models.Dto
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
