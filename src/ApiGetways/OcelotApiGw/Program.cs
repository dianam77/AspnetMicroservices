using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// فایل‌های پیکربندی Ocelot
builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// لاگ‌گیری
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// روی پورت 8010 گوش کن
builder.WebHost.UseUrls("http://*:8010");   // یا ConfigureKestrel، هر کدام راحت‌ترید

// DI
builder.Services.AddOcelot()
                .AddCacheManager(x => x.WithDictionaryHandle());

var app = builder.Build();

// روت / : فقط «hello» چاپ شود
app.MapGet("/", () => Results.Text("hello", "text/plain; charset=utf-8"));

// بقیۀ درخواست‌ها را به Ocelot بده
await app.UseOcelot();
await app.RunAsync();
