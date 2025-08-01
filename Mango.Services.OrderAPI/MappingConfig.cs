﻿using AutoMapper;
using Mango.Services.OrderAPI.Extensions;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto.Cart;
using Mango.Services.OrderAPI.Models.Dto.Order;

namespace Mango.Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderHeaderDto, CartHeaderDto>()
                .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal))
                .ReverseMap();

                config.CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

                config.CreateMap<OrderDetailsDto, CartDetailsDto>();

                config.CreateMap<OrderHeaderDto, OrderHeader>()
               .Ignore(x => x.CreatedAt)
               .Ignore(x => x.UpdatedAt);

                config.CreateMap<OrderHeader, OrderHeaderDto>();

                config.CreateMap<OrderDetailsDto, OrderDetails>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
