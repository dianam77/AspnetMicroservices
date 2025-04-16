using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Discount.GRPC;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Configure Kestrel to use HTTP/2 for gRPC
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5004, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2; // gRPC uses HTTP/2
    });
    options.ListenAnyIP(5001); // HTTP port for REST/Swagger
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Redis cache config
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConnectionString = configuration.GetValue<string>("CacheSettings:ConnectionString")
        ?? throw new ArgumentNullException("CacheSettings:ConnectionString", "Redis connection string is missing.");
    options.Configuration = redisConnectionString;
});

builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(o =>
{
    o.Address = new Uri(configuration["GrpcSettings:DiscountUrl"]);
});

// Register custom services
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped<DiscountGrpcService>();
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddJsonFile("appsettings.Docker.json", optional: true) 
    .AddEnvironmentVariables();


var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();


app.Run();
