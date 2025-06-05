using CartService.API.repository.interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CartService.Test.Integration
    {
    internal class CartServiceWebApplicationFactoryWithFailingRepository : CartServiceWebApplicationFactory
        {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ICartRepository>();

                services.AddScoped<ICartRepository, FailingCartRepository>();
            });
            }
        }
    }
