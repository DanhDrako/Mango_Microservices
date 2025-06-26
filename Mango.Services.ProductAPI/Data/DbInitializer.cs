using Mango.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Data
{
    public class DbInitializer
    {
        public static void InitDb(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>()
                ?? throw new InvalidOperationException("Failed to retrieve store context");

            SeedData(context);
        }

        private static void SeedData(AppDbContext context)
        {
            context.Database.Migrate();
            if (context.Products.Any()) return;

            var products = new List<Product>
            {
                new()
                {
                    Name = "Samosa",
                    Price = 15,
                    Description = " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium.",
                    ImageUrl = "https://placehold.co/603x403",
                    Type = "Appetizer",
                    Brand = "Indian Snacks",
                    QuantityInStock = 100
                },
                new()
                {
                    Name = "Paneer Tikka",
                    Price = 13.99,
                    Description = " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium.",
                    ImageUrl = "https://placehold.co/602x402",
                    Type = "Appetizer",
                    Brand = "Indian Snacks",
                    QuantityInStock = 100
                },
                new()
                {
                    Name = "Sweet Pie",
                    Price = 10.99,
                    Description = " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium.",
                    ImageUrl = "https://placehold.co/601x401",
                    Type = "Dessert",
                    Brand = "Indian Sweets",
                    QuantityInStock = 100
                },
                new()
                {
                    Name = "Pav Bhaji",
                    Price = 15,
                    Description = " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium.",
                    ImageUrl = "https://placehold.co/600x400",
                    Type = "Dessert",
                    Brand = "Indian Sweets",
                    QuantityInStock = 100
                }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}
