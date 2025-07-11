using AutoMapper;
using log4net;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Models.Dto.Filters;
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
        private readonly IMapper _mapper;
        private readonly ResponseDto _response;
        private readonly IProductService _productService;
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ProductAPIController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
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
                _logger.Error("Error occurred while fetching products:", ex);
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
                _logger.Error($"Error occurred while fetching product with ID {id}:", ex);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Post(CreateProductDto productDto)
        {
            try
            {
                var result = await _productService.Create(productDto);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error while creating product.";
                    return _response;
                }
                _logger.Info($"Created a product with ID {result.ProductId}");
                _response.Result = _mapper.Map<CreateProductDto>(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while creating product:", ex);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Put(UpdateProductDto productDto)
        {
            try
            {
                var result = await _productService.Update(productDto);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error while updating product.";
                    return _response;
                }
                _logger.Info($"Updated a product with ID {result.ProductId}");
                _response.Result = _mapper.Map<UpdateProductDto>(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while updating product with ID {productDto.ProductId}:", ex);
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
                var result = await _productService.Delete(id);
                if (result == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error while deleting product.";
                    return _response;
                }
                _logger.Info($"Deleted a product with ID {id}");
                return _response;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while deleting product with ID {id}:", ex);
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
                _response.Result = result;
                return _response;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while fetching filters:", ex);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("edit-filters")]
        public async Task<ResponseDto> EditFilters(FilterInput filter)
        {
            try
            {
                var result = await _productService.EditFilters(filter);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error while editing filter.";
                    return _response;
                }
                _logger.Info($"Edited a filters successfully.");
                _response.Result = result;
                return _response;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while editing filters:", ex);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

    }
}
