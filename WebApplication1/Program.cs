using WebApplication1.Data.Customer;
using WebApplication1.Infrastructure.ConnectionFacory;
using WebApplication1.Infrastructure.Connector;
using WebApplication1.Middlewares.Extension;
using WebApplication1.Middlewares.Options;
using WebApplication1.Middlewares.Storage;
using WebApplication1.Middlewares.Validators;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddOptions<ApplicationOptions>()
                .Bind(builder.Configuration.GetSection(ApplicationOptions.Secrets));

builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>();
builder.Services.AddScoped<IDataAccess, DataAccessLayer>();
builder.Services.AddScoped<ICustomerDataAccess, CustomerDataAccess>();

//Registration for Middleware
builder.Services.AddOptions<RateLimitingOptions>()
                .Bind(builder.Configuration.GetSection(RateLimitingOptions.Config));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IRateLimiter, RateLimiter>();

builder.Services.AddSingleton<IStorage, DistributedStorage>();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapRazorPages();

app.Run();
