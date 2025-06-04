using Microsoft.EntityFrameworkCore;
using ProductService.API.Data;
using ProductService.API.Repository;
using ProductService.API.Repository.RepositoryInterfaces;
using ProductService.API.Services;
using ProductService.API.Services.serviceInterfaces;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddScoped<IProductService, ProductServiceImpl>();
builder.Services.AddScoped<IProductRepo, ProductRepositoryIMPL>();
builder.Services.AddScoped<IProductContentService, ProductContentServiceIMPL>();
builder.Services.AddScoped<IProductContentRepository, ProductContentRepositoryIMPL>();
builder.Services.AddScoped<IProductAttributeService, ProductAttributeServiceIMPL>();
builder.Services.AddScoped<IProductAttriuteRepository, ProductAttributeRepositoryIMPL>();
builder.Services.AddHttpClient<IProductService, ProductServiceImpl>();
builder.Services.Configure<ServiceUrls>(builder.Configuration.GetSection("ServiceUrls"));


// Database
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("productConnection")));

var serviceUrls = builder.Configuration.GetSection("ServiceUrls").Get<ServiceUrls>();
builder.WebHost
    .UseKestrel()
    .UseContentRoot(Directory.GetCurrentDirectory())
    .UseUrls(serviceUrls.RedirectUrl);


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    db.Database.Migrate();
}


using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider.GetRequiredService<IProductService>();
    await service.UpdateProductsFromAdapterAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
