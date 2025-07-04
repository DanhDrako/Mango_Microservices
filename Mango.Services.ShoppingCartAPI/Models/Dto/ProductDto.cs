﻿namespace Mango.Services.ShoppingCartAPI.Models.Dto
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public int QuantityInStock { get; set; }
        public string? ImageUrl { get; set; }
    }
}
