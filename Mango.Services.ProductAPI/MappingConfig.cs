using AutoMapper;
using Mango.Services.ProductAPI.Extensions;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Models.Dto.Filters;

namespace Mango.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<UpdateProductDto, Product>()
                .Ignore(x => x.CreatedAt)
                .Ignore(x => x.UpdatedAt)
                .Ignore(x => x.ImageLocalPath)
                .ReverseMap();
                config.CreateMap<CreateProductDto, Product>().ReverseMap();
                config.CreateMap<ProductDto, Product>().ReverseMap();
                config.CreateMap<CategoryDto, Category>().ReverseMap();
                config.CreateMap<BrandDto, Brand>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
