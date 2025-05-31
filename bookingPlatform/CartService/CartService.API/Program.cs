using CartService.API.Data;
using CartService.API.repository;
using CartService.API.repository.interfaces;
using CartService.API.services;
using CartService.API.services.interfaces;
using Microsoft.EntityFrameworkCore;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ICartService, CartServiceIMPL>();
builder.Services.AddScoped<ICartRepository, CartRepositoryIMPL>();
builder.Services.AddDbContext<CartDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("cartConnectionString")));
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<ICartService, CartServiceIMPL>()
    .AddTransientHttpErrorPolicy(policyBuilder =>
        policyBuilder.CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(30)));


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();

app.MapControllers();

app.Run();
