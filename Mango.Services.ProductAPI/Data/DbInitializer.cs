using Mango.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>()
                ?? throw new InvalidOperationException("Failed to retrieve store context");

            await SeedCateData(context);
            await SeedBrandData(context);
            await SeedProductData(context);
        }

        private static async Task SeedCateData(AppDbContext context)
        {
            context.Database.Migrate();
            if (context.Categories.Any()) return;
            var categories = new List<Category>
            {
                new() { Name = "Food" },
                new() { Name = "Water" }
            };
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        private static async Task SeedBrandData(AppDbContext context)
        {
            context.Database.Migrate();
            if (context.Brands.Any()) return;
            var brands = new List<Brand>
            {
                new() { Name = "KFC" },
                new() { Name = "Jollibee" }
            };
            context.Brands.AddRange(brands);
            await context.SaveChangesAsync();
        }

        private static async Task SeedProductData(AppDbContext context)
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
                    QuantityInStock = 100
                },
                new()
                {
                    Name = "Paneer Tikka",
                    Price = 13.99,
                    Description = " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium.",
                    ImageUrl = "https://placehold.co/602x402",
                    QuantityInStock = 100
                },
                new()
                {
                    Name = "Sweet Pie",
                    Price = 10.99,
                    Description = " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium.",
                    ImageUrl = "https://placehold.co/601x401",
                    QuantityInStock = 100
                },
                new()
                {
                    Name = "Pav Bhaji",
                    Price = 15,
                    Description = " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium.",
                    ImageUrl = "https://placehold.co/600x400",
                    QuantityInStock = 100
                }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}
