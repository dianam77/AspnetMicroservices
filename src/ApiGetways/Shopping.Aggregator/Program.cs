using Shopping.Aggregator.Services;
using System.IO;
using Microsoft.Extensions.Configuration;  

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                    optional: true, reloadOnChange: true)  
       .AddEnvironmentVariables();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ICatalogService, CatalogService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogUrl"]!);
});

builder.Services.AddHttpClient<IBasketService, BasketService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketUrl"]!);
});

builder.Services.AddHttpClient<IOrderService, OrderService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderingUrl"]!);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
