using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;

public class OrderContext : DbContext
{
    public OrderContext(DbContextOptions<OrderContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Explicitly set precision and scale for TotalPrice property in Order entity
        modelBuilder.Entity<Order>()
            .Property(o => o.TotalPrice)
            .HasColumnType("decimal(18,2)") // Define precision and scale
            .IsRequired(); // Optionally, you can make TotalPrice required if necessary
    }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = DateTime.Now;
                    entry.Entity.CreatedBy = "swg"; // Ideally, set this dynamically (e.g., from the logged-in user)
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedDate = DateTime.Now;
                    entry.Entity.LastModifiedBy = "swg"; // Set dynamically as well
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
