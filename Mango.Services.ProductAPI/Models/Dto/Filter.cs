using Mango.Services.ProductAPI.Models.Dto.Filters;

namespace Mango.Services.ProductAPI.Models.Dto
{
    public class Filter
    {
        public IEnumerable<CategoryDto> Categories { get; set; }
        public IEnumerable<BrandDto> Brands { get; set; }
    }
}
