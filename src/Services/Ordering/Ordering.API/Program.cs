using EventBus.Messages.Common;
using MassTransit;
using MassTransit.Definition;
using Ordering.API.Extentions;
using Ordering.API.Mapping;
using Ordering.Application;
using Ordering.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddScoped<BasketCheckoutConsumer>();

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BasketCheckoutConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, e =>
        {
            e.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
        });
    });
});

builder.Services.AddMassTransitHostedService();

builder.Services.AddAutoMapper(typeof(OrderingProfile));


builder.Services.AddHealthChecks();

var app = builder.Build();

try
{
    await app.MigrateDatabaseAsync<OrderContext>((context, services) =>
    {
        var logger = services.GetRequiredService<ILogger<OrderContextSeed>>();
        return OrderContextSeed.SeedAsync(context, logger);
    });
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating the database.");
    throw;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
