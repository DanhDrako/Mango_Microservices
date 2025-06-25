using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.RequestHelpers;
using Mango.Services.ProductAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly ResponseDto _response;
        private readonly IProductService _productService;

        public ProductAPIController(IProductService productService)
        {
            _response = new();
            _productService = productService;
        }

        [HttpGet]
        public async Task<ResponseDto> Get([FromQuery] ProductParams productParams)
        {
            try
            {
                var result = await _productService.GetProducts(productParams);
                if (result == null || !result.Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "No products found.";
                    return _response;
                }
                _response.Result = result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("{id:int}")]
        public async Task<ResponseDto> Get(int id)
        {
            try
            {
                var result = await _productService.GetProductById(id);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Product not found.";
                    return _response;
                }
                _response.Result = result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Post(ProductDto productDto)
        {
            try
            {
                var result = await _productService.CreateUpdateProduct(productDto);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error while creating product.";
                    return _response;
                }
                _response.Result = result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Put(ProductDto productDto)
        {
            try
            {
                var result = await _productService.CreateUpdateProduct(productDto);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error while updating product.";
                    return _response;
                }
                _response.Result = result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Delete(int id)
        {
            try
            {
                var result = await _productService.DeleteProduct(id);
                if (result == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error while deleting product.";
                    return _response;
                }
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("filters")]
        public async Task<ResponseDto> GetFilters()
        {
            try
            {
                var result = await _productService.GetFilters();
                if (result.Types.Count <= 0 || result.Brands.Count <= 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error while get filters.";
                    return _response;
                }
                _response.Result = result;
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
