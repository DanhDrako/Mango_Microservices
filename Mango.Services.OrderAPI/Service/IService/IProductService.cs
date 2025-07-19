using Mango.Services.OrderAPI.Models.Dto;

namespace Mango.Services.OrderAPI.Service.IService
{
    public interface IProductService
    {
        Task<ProductDto> GetProduct(int productId);
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
