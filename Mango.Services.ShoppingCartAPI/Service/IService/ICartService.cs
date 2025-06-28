

using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Service.IService
{
    public interface ICartService
    {
        Task<CartDto> GetCart(string userId);
        Task<bool> ApplyCoupon(CartDto cartDto);
        Task<object> EmailCartRequest(CartDto cartDto);
        Task<CartDto> CartUpsert(CartDto cartDto);
        Task<bool> RemoveCart(int cartDetailsId);
    }
}
