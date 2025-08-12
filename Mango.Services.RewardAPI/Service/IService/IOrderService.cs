using Mango.Services.RewardAPI.Models.Dto.Order;

namespace Mango.Services.RewardAPI.Service.IService
{
    public interface IOrderService
    {
        Task<OrderHeaderDto> GetOrder(string paymentIntentId);
    }
}
