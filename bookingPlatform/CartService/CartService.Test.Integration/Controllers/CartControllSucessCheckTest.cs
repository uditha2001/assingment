using CartService.API.Data;
using CartService.API.DTO;
using CartService.API.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Test.Integration.Controllers
    {
    public class CartControllSucessCheckTest
        {
        private long SeedTestData(CartServiceWebApplicationFactory application)
            {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CartDbContext>();

            dbContext.Database.ExecuteSqlRaw("DELETE FROM cartItems");

            var entity = new CartItemEntity
                {
                UserId = 1,
                ProductId = 101,
                Quantity = 2,
                itemTotalPrice = 50.00m
                };
            dbContext.cartItems.Add(entity);
            dbContext.SaveChanges();
            return entity.cartItemId;
            }

        [Fact]
        public async Task GetCartByUserId_ReturnsCart_WhenErrorNotOccur()
            {
            // Arrange
            var application = new CartServiceWebApplicationFactory();
            SeedTestData(application);
            var client = application.CreateClient();
            long testUserId = 1;

            // Act
            var response = await client.GetAsync($"/api/v1/cart/{testUserId}");

            // Assert
            response.EnsureSuccessStatusCode();

            var cartItems = await response.Content.ReadFromJsonAsync<List<CartItemEntity>>();
            Assert.NotNull(cartItems);
            Assert.NotEmpty(cartItems);

            // Check all returned items have the expected user ID
            Assert.All(cartItems, item => Assert.Equal(testUserId, item.UserId));
            }

        [Fact]
        public async Task AddItemToCart_ReturnsOk_WhenItemIsAddedSuccessfully()
            {
            var application = new CartServiceWebApplicationFactory();
            var client = application.CreateClient();

            var newCartItem = new CartItemDTO
                {
                userId = 1,
                ProductId = 102,
                Quantity = 3,
                itemTotalPrice = 75.00m
                };

            var response = await client.PostAsJsonAsync("/api/v1/cart/add", newCartItem);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<bool>();
            Assert.True(result);
            }
        [Fact]
        public async Task UpdateCartItemQuantity_ReturnsOk_WhenAllItemsUpdatedSuccessfully()
            {
            var application = new CartServiceWebApplicationFactory();
            long cartItemId = SeedTestData(application); 
            var client = application.CreateClient();

            var updates = new List<UpdateQuantityDTO>
    {
        new UpdateQuantityDTO
        {
            CartItemId = cartItemId,
            NewQuantity = 5,
            newTotalPrice = 125.00m
        }
    };
            
            var response = await client.PutAsJsonAsync("/api/v1/cart/update-quantity", updates);

            response.EnsureSuccessStatusCode();
            var message = await response.Content.ReadAsStringAsync();
            Assert.Contains("All quantities updated successfully", message);
            }
 



        }
    }
