using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class CartService : ICartService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;

        public CartService(IMapper mapper, AppDbContext db, IProductService productService, ICouponService couponService)
        {
            _mapper = mapper;
            _db = db;
            _productService = productService; // Initialize IProductService
            _couponService = couponService; // Initialize ICouponService
        }

        public async Task<bool> ApplyCoupon(CartDto cartDto)
        {
            var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
            cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
            _db.CartHeaders.Update(cartFromDb);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<CartDto> CartUpsert(CartDto cartDto)
        {
            var cartHeaderFromDb = await _db.CartHeaders
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
            if (cartHeaderFromDb == null)
            {
                // user adds first item to the cart
                // 1. create a new cart header
                CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                _db.CartHeaders.Add(cartHeader);
                await _db.SaveChangesAsync();

                // 2. create a new cart details item
                List<CartDetails> cartDetails = _mapper.Map<List<CartDetails>>(cartDto.CartDetails);
                foreach (var detail in cartDetails)
                {
                    detail.CartHeaderId = cartHeader.CartHeaderId; // Associate the details with the new header
                    _db.CartDetails.Add(detail);
                }
                await _db.SaveChangesAsync();
            }
            else
            {
                // if header is not null

                // check if the cart details are null
                if (cartDto.CartDetails == null) throw new Exception("Cart details cannot be null");

                // user adds a new item to the cart or updates an existing item
                foreach (var detail in cartDto.CartDetails)
                {
                    var productDto = await _productService.GetProduct(detail.ProductId) ?? throw new Exception("Product not found");

                    // check if details has same product
                    var cartDetailsFromDb = await _db.CartDetails
                        .AsNoTracking()
                        .FirstOrDefaultAsync
                        (u => u.ProductId == productDto.ProductId &&
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                    // user adds a new item in the shopping cart (already has few other items a cart)
                    if (cartDetailsFromDb == null)
                    {
                        CartDetails cartDetail = _mapper.Map<CartDetails>(detail);
                        cartDetail.CartHeaderId = cartHeaderFromDb.CartHeaderId; // Associate the details with the new header
                        _db.CartDetails.Add(cartDetail);
                        await _db.SaveChangesAsync();
                    }
                    // user updates the quantity of an existing item in the cart
                    else
                    {
                        cartDetailsFromDb.Count += detail.Count; // Update the count
                        _db.CartDetails.Update(cartDetailsFromDb);
                        _db.CartHeaders.Update(cartHeaderFromDb);
                        await _db.SaveChangesAsync();
                    }
                }
            }
            return cartDto;
        }

        public Task<object> EmailCartRequest(CartDto cartDto)
        {
            throw new NotImplementedException();
        }

        public async Task<CartDto> GetCart(string userId)
        {
            CartDto cart = new()
            {
                CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.FirstOrDefault(u => u.UserId == userId)),
            };
            cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(
                _db.CartDetails.Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));

            var productDtos = await _productService.GetProducts();
            foreach (var item in cart.CartDetails)
            {
                item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
            }

            // apply coupon if any
            if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
            {
                CouponDto coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                {
                    cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                    cart.CartHeader.Discount = coupon.DiscountAmount;
                }
            }

            return cart;
        }

        public async Task<bool> RemoveCart(int cartDetailsId)
        {
            CartDetails cartDetails = _db.CartDetails.First(u => u.CartDetailsId == cartDetailsId);

            int totalCountOfCartItem = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
            _db.CartDetails.Remove(cartDetails);

            if (totalCountOfCartItem == 1)
            {
                var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(
                    u => u.CartHeaderId == cartDetails.CartHeaderId);

                if (cartHeaderToRemove == null) return false;

                _db.CartHeaders.Remove(cartHeaderToRemove);
            }
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
