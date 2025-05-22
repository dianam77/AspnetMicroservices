using AspnetRunBasics.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;          // ← برای LINQ در روش سراسری

namespace AspnetRunBasics.Data
{
    public class AspnetRunContext : DbContext
    {
        public AspnetRunContext(DbContextOptions<AspnetRunContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 💡 ساده‌ترین حالت: فقط فیلدهای حساس را صریح بنویسیم
            modelBuilder.Entity<Product>()
                        .Property(p => p.Price)
                        .HasPrecision(18, 2);

            modelBuilder.Entity<CartItem>()
                        .Property(ci => ci.Price)
                        .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                        .Property(o => o.TotalPrice)
                        .HasPrecision(18, 2);

            /*
            // 🔄 روش عمومی (اختیاری): همهٔ decimal‌ها 18,2 بشوند
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                                   .SelectMany(t => t.GetProperties())
                                   .Where(p => p.ClrType == typeof(decimal)
                                            || p.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }
            */
        }
    }
}
