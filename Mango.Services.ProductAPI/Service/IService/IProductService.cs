using Mango.Services.ProductAPI.Models.Dto;

namespace Mango.Services.ProductAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
        Task<ProductDto> GetProductById(int id);
        Task<ProductDto> CreateUpdateProduct(ProductDto productDto);
        Task<int> DeleteProduct(int id);
    }
}
