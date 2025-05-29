using AspNetRunBasic.Services;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/dataprotectionkeys"))
    .SetApplicationName("aspnetrunbasic");

builder.Services.AddRazorPages();

builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
    c.BaseAddress = new Uri(configuration["ApiSettings:CatalogUrl"]!));

builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
    c.BaseAddress = new Uri(configuration["ApiSettings:BasketUrl"]!));

builder.Services.AddHttpClient<IOrderService, OrderService>(c =>
    c.BaseAddress = new Uri(configuration["ApiSettings:OrderingUrl"]!));

var app = builder.Build();

if (app.Environment.IsEnvironment("Docker"))
{
    app.UseExceptionHandler("/Error");   
}
else
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();           
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
