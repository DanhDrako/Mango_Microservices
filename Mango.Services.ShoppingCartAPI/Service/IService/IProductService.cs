using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        Task<ProductDto> GetProduct(int productId);
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
