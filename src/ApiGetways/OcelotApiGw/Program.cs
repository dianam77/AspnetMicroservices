using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.WebHost.UseUrls();  


builder.Services.AddOcelot()
                .AddCacheManager(x => x.WithDictionaryHandle());

var app = builder.Build();

app.MapGet("/", () => Results.Text("hello", "text/plain; charset=utf-8"));

// بقیۀ درخواست‌ها را به Ocelot بده
await app.UseOcelot();
await app.RunAsync();
