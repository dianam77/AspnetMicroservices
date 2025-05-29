using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Ordering.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrderContext>
    {
        public OrderContext CreateDbContext(string[] args)
        {
          
            var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Ordering.API"));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<OrderContext>();
            var connectionString = configuration.GetConnectionString("OrderingConnectionString");

            optionsBuilder.UseSqlServer(connectionString);

            return new OrderContext(optionsBuilder.Options);
        }
    }
}
