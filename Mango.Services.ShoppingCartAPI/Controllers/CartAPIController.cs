﻿using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dto.Cart;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly ResponseDto _response;
        private readonly ICartService _cartService;

        public CartAPIController(ICartService cartService)
        {
            _response = new();
            _cartService = cartService;
        }

        [HttpGet("{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                var cart = await _cartService.GetCart(userId);
                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto> ApplyCoupon(CartHeaderDto cartDto)
        {
            try
            {
                var result = await _cartService.ApplyCoupon(cartDto);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Failed to apply coupon.";
                    return _response;
                }
                _response.Result = result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest(CartHeaderDto cartDto)
        {
            try
            {
                var result = await _cartService.EmailCartRequest(cartDto);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Failed to send cart email.";
                    return _response;
                }
                _response.Result = result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(InputCartDto inputCartDto)
        {
            try
            {
                var result = await _cartService.CartUpsert(inputCartDto);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Failed to upsert cart.";
                    return _response;
                }
                _response.Result = result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpDelete("RemoveCart")]
        public async Task<ResponseDto> RemoveCart(InputCartDto inputCartDto)
        {
            try
            {
                var result = await _cartService.RemoveCart(inputCartDto);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Failed to remove cart item.";
                    return _response;
                }
                _response.Result = "Cart item removed successfully.";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
