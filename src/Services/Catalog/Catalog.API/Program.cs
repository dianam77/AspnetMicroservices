using Catalog.API.Data;
using Catalog.API.Repositories;
using Catalog.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// 🧠 Load config
var configuration = builder.Configuration;
var connectionString = Environment.GetEnvironmentVariable("DatabaseSettings__ConnectionString")
                       ?? configuration["DatabaseSettings:ConnectionString"];
var databaseName = Environment.GetEnvironmentVariable("DatabaseSettings__DatabaseName")
                   ?? configuration["DatabaseSettings:DatabaseName"];

// ✅ Optional MongoDB connection test
try
{
    var client = new MongoClient(connectionString);
    var database = client.GetDatabase(databaseName);
    Console.WriteLine($"✅ Connected to MongoDB at {connectionString}.");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ MongoDB connection failed: {ex.Message}");
    throw;
}

// 🧩 Dependency Injection
builder.Services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(connectionString)
);
builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// 📦 Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🧪 Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ICatalogContext>();
    CatalogContextSeed.SeedData(context.Products);
}

// 🌐 Middleware
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok("Healthy"));

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
