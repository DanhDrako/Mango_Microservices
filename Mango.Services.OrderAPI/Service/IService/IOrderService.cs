using Mango.Services.OrderAPI.Models.Dto;

namespace Mango.Services.OrderAPI.Service.IService
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderHeaderDto>> GetOrdersByUserId(string? userId, bool isAdmin);
        Task<OrderHeaderDto> GetOrderById(int orderHeaderId);
        Task<OrderHeaderDto> CreateOrder(CartHeaderDto cartHeader);
        Task<OrderHeaderDto> UpdateOrderStatus(int orderHeaderId, string status);
    }
}
