using AdapterFactory.Adapters;
using AdapterFactory.Service;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Polly;

var builder = WebApplication.CreateBuilder(args);
var urls = builder.Configuration.GetSection("ServiceUrls").Get<ServiceUrls>();
Console.WriteLine($"AdapterFactoryService: {urls.ProductService}");
Console.WriteLine($"RedirectUrl: {urls.CdeMockService}");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<AbcAdapter>();
builder.Services.AddTransient<CdeAdapter>();
builder.Services.AddHttpClient<IAdapter, AbcAdapter>();
builder.Services.AddHttpClient<IAdapter, CdeAdapter>();
builder.Services.AddSingleton<IAdapterFactory, AdapterFactoryServiceIMPL>();
builder.Services.Configure<ServiceUrls>(
    builder.Configuration.GetSection("ServiceUrls"));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IAdapterFactory, AdapterFactoryServiceIMPL>()
    .AddTransientHttpErrorPolicy(policyBuilder =>
        policyBuilder.CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(30)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();

app.MapControllers();

app.Run();
