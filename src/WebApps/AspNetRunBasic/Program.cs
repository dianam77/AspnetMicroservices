using AspnetRunBasics.Data;
using AspnetRunBasics.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddDbContext<AspnetRunContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("AspnetRunConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));


// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

// Add Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();

    try
    {
        var context = services.GetRequiredService<AspnetRunContext>();
        AspnetRunContextSeed.SeedAsync(context, loggerFactory).Wait();
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger("App");
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
