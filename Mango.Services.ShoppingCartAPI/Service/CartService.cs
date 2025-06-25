using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class CartService : ICartService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor; // Add IHttpContextAccessor for accessing HttpContext

        public CartService(IMapper mapper, AppDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _db = db;
            _httpContextAccessor = httpContextAccessor; // Initialize IHttpContextAccessor
        }

        public Task<object> ApplyCoupon(CartDto cartDto)
        {
            throw new NotImplementedException();
        }

        public Task<CartDto> CartUpsert(CartDto cartDto)
        {
            throw new NotImplementedException();
        }

        public Task<object> EmailCartRequest(CartDto cartDto)
        {
            throw new NotImplementedException();
        }

        public Task<CartHeaderDto> GetCartByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveCart(int cartDetailsId)
        {
            throw new NotImplementedException();
        }
    }
}
