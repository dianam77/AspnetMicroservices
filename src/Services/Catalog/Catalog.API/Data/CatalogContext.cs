using Catalog.API.Entities;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public IMongoCollection<Product> Products { get; }

        public CatalogContext(IConfiguration configuration)
        {
            try
            {
                var client = new MongoClient(configuration["DatabaseSettings:ConnectionString"]);
                var database = client.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);
                Products = database.GetCollection<Product>(configuration["DatabaseSettings:CollectionName"]);

                // Seed database if necessary
                //CatalogContextSeed.SeedData(Products);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("MongoDB connection failed.", ex);
            }
        }
    }
}