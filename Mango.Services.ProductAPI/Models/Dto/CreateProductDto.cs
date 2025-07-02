namespace Mango.Services.ProductAPI.Models.Dto
{
    public class CreateProductDto
    {
        public required string Name { get; set; }
        public double Price { get; set; }
        public string? Description { get; set; }
        public int QuantityInStock { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
        public IFormFile? File { get; set; }
        public string? CategoryId { get; set; }
        public string? BrandId { get; set; }
    }
}
