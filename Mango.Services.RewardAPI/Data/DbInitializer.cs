using Microsoft.EntityFrameworkCore;

namespace Mango.Services.RewardAPI.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>()
                ?? throw new InvalidOperationException("Failed to retrieve db context");

            // migration
            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }
        }
    }
}
