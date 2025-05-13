using Basket.API.GrpcServices;
using Basket.API.Mapper;
using Basket.API.Repositories;
using Discount.GRPC;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Load configuration first
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddJsonFile("appsettings.Docker.json", optional: true)
    .AddEnvironmentVariables();

var configuration = builder.Configuration;

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registering MassTransit for Event Publishing
builder.Services.AddMassTransit(configure =>
{
    configure.AddPublishMessageScheduler();

    configure.AddDelayedMessageScheduler();

    configure.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(configuration["EventBusSettings:HostAddress"]);

        cfg.ConfigureEndpoints(ctx);
    });
});


builder.Services.AddAutoMapper(typeof(BasketProfile));
builder.Services.AddMassTransitHostedService();

// Redis Configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConnectionString = configuration.GetValue<string>("CacheSettings:ConnectionString")
        ?? throw new ArgumentNullException("CacheSettings:ConnectionString", "Redis connection string is missing.");
    options.Configuration = redisConnectionString;
});

// gRPC Configuration (Discount service)
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(o =>
{
    o.Address = new Uri(configuration["GrpcSettings:DiscountUrl"]);
});
builder.Services.AddScoped<DiscountGrpcService>();

// Register IBasketRepository and its implementation
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
