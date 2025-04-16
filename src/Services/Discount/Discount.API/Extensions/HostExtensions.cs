using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating PostgreSQL database.");

                    using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    using var command = new NpgsqlCommand { Connection = connection };

                    // Drop the Coupon table if it exists
                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();

                    // Create the Coupon table with corrected SQL syntax
                    command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS Coupon (
                                Id SERIAL PRIMARY KEY,
                                ProductName VARCHAR(24) NOT NULL,
                                Description TEXT,
                                Amount INT
                            );
                        ";
                    command.ExecuteNonQuery();


                    // Insert default records
                    command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('Iphone X', 'Iphone Discount', 150);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('Samsung 10', 'Samsung Discount', 100);";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Migration of PostgreSQL database completed successfully.");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database.");

                    if (retryForAvailability < 5)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000); // Consider using Task.Delay in an async context
                        return MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }
            }

            return host;
        }
    }
}
