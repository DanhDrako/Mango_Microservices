using Mango.Services.PaymentAPI.Models.Dto.Order;

namespace Mango.Services.PaymentAPI.Service.IService
{
    public interface IOrderService
    {
        Task<OrderHeaderDto> GetOrder(int orderHeaderId);
        Task<OrderHeaderDto> UpdateHeaderDto(OrderHeaderDto orderHeaderDto);
    }
}
