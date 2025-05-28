using AspNetRunBasic.Services;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ---------- Services ----------
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

// ---------- App ----------
var app = builder.Build();

// خطاهای برنامه فقط در محیط‌های غیر Docker به HTTPS ریدایرکت می‌شود
if (app.Environment.IsEnvironment("Docker"))
{
    // در کانتینر فقط HTTP اجرا می‌کنیم
    app.UseExceptionHandler("/Error");   // صفحه خطا را همچنان نگه می‌داریم
    // HSTS و HTTPS را عمداً فعال نمی‌کنیم
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
