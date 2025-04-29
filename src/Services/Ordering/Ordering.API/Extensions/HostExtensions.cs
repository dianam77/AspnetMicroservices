using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Ordering.API.Extentions
{
    public static class HostExtensions
    {
        public static async Task<IHost> MigrateDatabaseAsync<TContext>(this IHost host,
            Func<TContext, IServiceProvider, Task> seeder, int retry = 0) where TContext : DbContext
        {
            int retryForAvailability = retry;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();
                try
                {
                    logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

                    await InvokeSeederAsync(seeder, context, services);
                    logger.LogInformation("Successfully migrated and seeded database associated with context {DbContextName}", typeof(TContext).Name);
                }
                catch (SqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        await Task.Delay(2000); // Use Task.Delay instead of Thread.Sleep
                        await MigrateDatabaseAsync<TContext>(host, seeder, retryForAvailability);
                    }
                }
            }
            return host;
        }

        private static async Task InvokeSeederAsync<TContext>(Func<TContext, IServiceProvider, Task> seeder,
            TContext context, IServiceProvider services) where TContext : DbContext
        {
            await context.Database.MigrateAsync(); // Ensure async database migration
            await seeder(context, services); // Invoke async seeder
        }
    }
}
