using AutoMapper;
using log4net;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Models.Dto.Cart;
using Mango.Services.OrderAPI.Models.Dto.Order;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    [Authorize]
    public class OrderAPIController : ControllerBase
    {
        private readonly ResponseDto _response;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public OrderAPIController(IOrderService orderService, IMapper mapper)
        {
            _response = new();
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ResponseDto> GetOrderByUserId(OrderStatus? status, string? userId = "")
        {
            try
            {
                bool isAdmin = User.IsInRole(SD.RoleAdmin);

                var orders = await _orderService.GetOrdersByUserId(status, userId, isAdmin);
                if (orders == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No orders found.";
                    return _response;
                }
                _response.Result = orders;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while fetching order:", ex);
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
                var order = await _orderService.GetOrderById(id);
                if (order == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Fetch order failed.";
                    return _response;
                }
                _response.Result = _mapper.Map<OrderHeaderDto>(order);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while fetching order:", ex);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        public async Task<ResponseDto> CreateOrder(CartHeaderDto cartHeader)
        {
            try
            {
                var order = await _orderService.CreateOrder(cartHeader);
                if (order == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Order creation failed.";
                    return _response;
                }
                _logger.Info($"Order created successfully with Id: {order.OrderHeaderId}" +
                    $" paymentIntentId: {order.PaymentIntentId}");
                _response.Result = order;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while creating order:", ex);
                _response.IsSuccess = false;
                _response.Message = $"Error creating order: {ex.Message}";
            }
            return _response;
        }

        [HttpPut]
        public async Task<ResponseDto> UpdateOrder(OrderHeaderDto orderHeaderDto)
        {
            try
            {
                var order = await _orderService.UpdateOrder(orderHeaderDto);
                if (order == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Order updation failed.";
                    return _response;
                }
                _logger.Info($"Order updated successfully with Id: {order.OrderHeaderId} " +
                    $"paymentIntentId: {order.PaymentIntentId}");
                _response.Result = order;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while updating order:", ex);
                _response.IsSuccess = false;
                _response.Message = $"Error updating order: {ex.Message}";
            }
            return _response;
        }
    }
}
