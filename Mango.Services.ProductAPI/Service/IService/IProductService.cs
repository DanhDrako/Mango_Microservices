using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Models.Dto.Filters;
using Mango.Services.ProductAPI.RequestHelpers;

namespace Mango.Services.ProductAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts(ProductParams productParams);
        Task<ProductDto> GetProductById(int id);
        Task<CreateProductDto> Create(CreateProductDto productDto);
        Task<UpdateProductDto> Update(UpdateProductDto productDto);
        Task<int> Delete(int id);
        Task<Filter> GetFilters();
        Task<bool> EditFilters(FilterInput filter);
    }
}
