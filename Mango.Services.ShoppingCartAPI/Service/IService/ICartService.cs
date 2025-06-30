using Mango.Services.ShoppingCartAPI.Models.Dto.Cart;

namespace Mango.Services.ShoppingCartAPI.Service.IService
{
    public interface ICartService
    {
        Task<CartHeaderDto?> GetCart(string userId);
        Task<bool> ApplyCoupon(CartHeaderDto cartDto);
        Task<object> EmailCartRequest(CartHeaderDto cartDto);
        Task<CartHeaderDto> CartUpsert(InputCartDto inputCartDto);
        Task<bool> RemoveCart(InputCartDto inputCartDto);
    }
}
