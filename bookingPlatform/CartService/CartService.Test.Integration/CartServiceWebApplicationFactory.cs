using CartService.API.Data;
using CartService.API.repository;
using CartService.API.repository.interfaces;
using CartService.API.services;
using CartService.API.services.interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CartService.Test")]

namespace CartService.Test.Integration
    {
    internal class CartServiceWebApplicationFactory : WebApplicationFactory<Program>
        {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
            {

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                 d => d.ServiceType == typeof(DbContextOptions<CartDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);
                var connectionString = GetConnectionString();
                services.AddSqlServer<CartDbContext>(connectionString);
                var dbContext = CreateDbContext(services);
            });
            }

        private static string GetConnectionString()
            {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("cartConnectionString");

            if (string.IsNullOrEmpty(connectionString))
                {
                throw new InvalidOperationException("Connection string 'cartConnectionString' not found.");
                }
            return connectionString;
            }

        private static CartDbContext CreateDbContext(IServiceCollection services)
            {
            var serviceProvider = services.BuildServiceProvider();
            var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            return dbContext;
            }
        }
    }
