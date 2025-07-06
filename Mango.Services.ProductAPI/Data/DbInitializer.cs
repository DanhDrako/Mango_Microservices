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

            // migration for category, brand, product
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            await SeedCatesData(context);
            await SeedBrandsData(context);
            await SeedProductsData(context);
            await context.SaveChangesAsync();
        }

        private static async Task SeedCatesData(AppDbContext context)
        {
            if (context.Categories.Any()) return;
            var categories = new List<Category>
            {
                new() { Name = "Food" },
                new() { Name = "Water" }
            };
            await context.Categories.AddRangeAsync(categories);
        }

        private static async Task SeedBrandsData(AppDbContext context)
        {
            if (context.Brands.Any()) return;
            var brands = new List<Brand>
            {
                new() { Name = "KFC" },
                new() { Name = "Jollibee" }
            };
            await context.Brands.AddRangeAsync(brands);
        }

        private static async Task SeedProductsData(AppDbContext context)
        {
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

            await context.Products.AddRangeAsync(products);
        }
    }
}
