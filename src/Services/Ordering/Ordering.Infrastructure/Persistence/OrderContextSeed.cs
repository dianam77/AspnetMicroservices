using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;

public class OrderContextSeed
{
    public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
    {
        var userExists = await orderContext.Orders
            .AnyAsync(o => o.UserName == "swg");

        if (!userExists)
        {
            var orders = GetPreconfiguredOrders();

            foreach (var order in orders)
            {
                if (string.IsNullOrEmpty(order.CreatedBy))
                {
                    order.CreatedBy = "Seeder"; 
                }

                if (string.IsNullOrEmpty(order.LastModifiedBy))
                {
                    order.LastModifiedBy = "Seeder"; 
                }

                if (order.CreatedDate == default)
                {
                    order.CreatedDate = DateTime.UtcNow; 
                }

                if (order.LastModifiedDate == default)
                {
                    order.LastModifiedDate = DateTime.UtcNow; 
                }
            }

            orderContext.Orders.AddRange(orders);
            await orderContext.SaveChangesAsync();

            logger.LogInformation("Seed database associated with context {DbContextName}", typeof(OrderContext).Name);
        }
        else
        {
            logger.LogInformation("Data already exists with the UserName 'swg'. No seeding performed.");
        }
    }

    private static IEnumerable<Order> GetPreconfiguredOrders()
    {
        return new List<Order>
        {
            new Order
            {
                UserName = "swg",
                FirstName = "Mehmet",
                LastName = "Ozkaya",
                EmailAddress = "ozozkme@gmail.com",
                Country = "Turkey",
                AddressLine = "123 Main St",
                State = "Istanbul",
                ZipCode = "34000",
                TotalPrice = 350,
                CardName = "Mehmet Ozkaya",
                CardNumber = "1234567890123456",
                Expiration = "12/26",
                CVV = "123",
                PaymentMethod = 1,
                CreatedBy = "Seeder", 
                CreatedDate = DateTime.UtcNow, 
                LastModifiedBy = "Seeder", 
                LastModifiedDate = DateTime.UtcNow 
            }
        };
    }
}
