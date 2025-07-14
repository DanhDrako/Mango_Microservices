using Mango.Services.EmailAPI.Models.Dto.Cart;

namespace Mango.Services.EmailAPI.Service.IService
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartHeaderDto cartDto);
        Task EmailRegisterUserAndLog(string email);
    }
}
