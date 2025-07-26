using Mango.Services.OrderAPI.Models.Dto.Product;

namespace Mango.Services.OrderAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
