using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using OrderService.API.Data;
using OrderService.API.Repository;
using OrderService.API.Repository.Interface;
using OrderService.API.services;
using OrderService.API.Services.Interfaces;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IOrderService,OrderServiceIMPL>();
builder.Services.AddScoped<IOrderRepository, OrderRepositoryIMPL>();
builder.Services.AddHttpClient<IOrderService, OrderServiceIMPL>();
builder.Services.AddDbContext<OrderDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("orderDbString")));
builder.Services.AddHttpClient<IOrderService, OrderServiceIMPL>()
    .AddTransientHttpErrorPolicy(policyBuilder =>
        policyBuilder.CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(30)));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
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
