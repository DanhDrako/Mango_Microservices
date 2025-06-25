using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.RequestHelpers;

namespace Mango.Services.ProductAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts(ProductParams productParams);
        Task<ProductDto> GetProductById(int id);
        Task<ProductDto> CreateUpdateProduct(ProductDto productDto);
        Task<int> DeleteProduct(int id);
        Task<Filter> GetFilters();
    }
}
