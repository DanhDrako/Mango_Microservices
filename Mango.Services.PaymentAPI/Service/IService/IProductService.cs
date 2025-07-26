using Mango.Services.PaymentAPI.Models.Dto.Product;

namespace Mango.Services.PaymentAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
