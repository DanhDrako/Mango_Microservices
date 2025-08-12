using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto.Cart;
using Mango.Services.OrderAPI.Models.Dto.Order;
using Mango.Services.OrderAPI.Models.Dto.Payment;
using Mango.Services.OrderAPI.Utility;

namespace Mango.Services.OrderAPI.Service.IService
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderHeaderDto>> GetOrdersByUserId(OrderStatus? status, string? userId, bool isAdmin);
        Task<OrderHeader> GetOrderById(int orderHeaderId);
        Task<OrderHeader> GetOrderById(string paymentIntentId);
        Task<OrderHeaderDto> CreateOrder(CartHeaderDto cartHeader);
        Task<OrderHeaderDto> UpdateOrder(OrderHeaderDto orderHeaderDto);
        Task<OrderHeaderDto> UpdateOrderStatus(int orderHeaderId, string status);
        Task<bool> UpdateOrderStatus(PaymentQueueDto paymentQueueDto);
    }
}
