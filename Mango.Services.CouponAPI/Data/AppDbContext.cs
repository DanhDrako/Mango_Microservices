using Mango.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // need move it to initialDb
            //if (modelBuilder.Model.FindEntityType(typeof(Coupon))?.GetSeedData() != null)
            //{
            //    return;
            //}

            //modelBuilder.Entity<Coupon>().HasData(new Coupon
            //{
            //    CouponCode = "10OFF",
            //    DiscountAmount = 10,
            //    MinAmount = 20
            //});

            //modelBuilder.Entity<Coupon>().HasData(new Coupon
            //{
            //    CouponCode = "20OFF",
            //    DiscountAmount = 20,
            //    MinAmount = 40
            //});
        }
    }
}
