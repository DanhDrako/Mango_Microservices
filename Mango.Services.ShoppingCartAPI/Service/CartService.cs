﻿using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dto.Cart;
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

        public async Task<bool> ApplyCoupon(CartHeaderDto cartDto)
        {
            var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.UserId);
            cartFromDb.CouponCode = cartDto.CouponCode;
            _db.CartHeaders.Update(cartFromDb);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<CartHeaderDto> CartUpsert(InputCartDto inputCartDto)
        {
            // get cart
            var cart = await RetrieveCartByUserId(inputCartDto.UserId);

            // create cartHeader if it doesn't exist
            cart ??= CreateCartHeader(inputCartDto.UserId);

            // get product
            var product = await _productService.GetProduct(inputCartDto.ProductId);
            if (product == null) throw new Exception("Product not found: " + inputCartDto.ProductId);

            // add item to cart
            cart.AddItem(product, inputCartDto.Quantity);

            var result = await _db.SaveChangesAsync() > 0;
            if (!result) throw new Exception("Failed to save cart for user: " + inputCartDto.UserId);

            return _mapper.Map<CartHeaderDto>(cart);

        }

        public Task<object> EmailCartRequest(CartHeaderDto cartDto)
        {
            throw new NotImplementedException();
        }

        public async Task<CartHeaderDto?> GetCart(string userId)
        {
            var cart = _mapper.Map<CartHeaderDto>(await RetrieveCartByUserId(userId));

            if (cart == null || cart.CartDetails == null || cart.CartDetails.Count == 0)
            {
                return cart ?? null;
            }

            var productDtos = await _productService.GetProducts();
            foreach (var item in cart.CartDetails)
            {
                item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                if (item.Product != null)
                {
                    cart.CartTotal += (item.Quantity * item.Product.Price);
                }
            }

            // apply coupon if any
            if (!string.IsNullOrEmpty(cart.CouponCode))
            {
                CouponDto coupon = await _couponService.GetCoupon(cart.CouponCode);
                if (coupon != null && cart.CartTotal > coupon.MinAmount)
                {
                    cart.CartTotal -= coupon.DiscountAmount;
                    cart.Discount = coupon.DiscountAmount;
                }
            }

            return cart;
        }

        public async Task<bool> RemoveCart(InputCartDto inputCartDto)
        {
            var cart = await RetrieveCartByUserId(inputCartDto.UserId) ?? throw new Exception("Cart not found for user: " + inputCartDto.UserId);

            // remove item from basket
            cart.RemoveItem(inputCartDto.ProductId, inputCartDto.Quantity);

            var result = await _db.SaveChangesAsync() > 0;
            if (result) return true;
            return false;
        }

        private CartHeader CreateCartHeader(string userId)
        {
            // create cart
            CartHeader cartHeader = new()
            {
                UserId = userId,
            };
            // add cart to context
            _db.CartHeaders.Add(cartHeader);
            return cartHeader;
        }

        private async Task<CartHeader?> RetrieveCartByUserId(string userId)
        {
            return await _db.CartHeaders
                .Include(x => x.CartDetails)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}