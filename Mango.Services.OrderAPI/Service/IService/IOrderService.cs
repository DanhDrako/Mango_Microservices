using Mango.Services.OrderAPI.Models.Dto.Cart;
using Mango.Services.OrderAPI.Models.Dto.Order;
using Mango.Services.OrderAPI.Utility;

namespace Mango.Services.OrderAPI.Service.IService
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderHeaderDto>> GetOrdersByUserId(OrderStatus? status, string? userId, bool isAdmin);
        Task<OrderHeaderDto> GetOrderById(int orderHeaderId);
        Task<OrderHeaderDto> CreateOrder(CartHeaderDto cartHeader);
        Task<OrderHeaderDto> UpdateOrder(OrderHeaderDto orderHeaderDto);
        Task<OrderHeaderDto> UpdateOrderStatus(int orderHeaderId, string status);
    }
}
