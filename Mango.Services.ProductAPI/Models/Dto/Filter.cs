namespace Mango.Services.ProductAPI.Models.Dto
{
    public class Filter
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
    }
}
