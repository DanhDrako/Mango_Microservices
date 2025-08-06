using Mango.Services.PaymentAPI.Models.Dto.Payment;

namespace Mango.Services.PaymentAPI.Service.IService
{
    public interface IPaymentService
    {
        Task<PaymentDto> CreateOrUpdatePayment(PaymentDto paymentDto);
        Task<bool> ProcessPayment(string json);
    }
}
