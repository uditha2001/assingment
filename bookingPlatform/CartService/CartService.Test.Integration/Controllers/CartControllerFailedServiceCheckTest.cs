using CartService.API.Data;
using CartService.API.DTO;
using CartService.API.Model.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace CartService.Test.Integration.Controllers
    {
    public class CartControllerFailedServiceCheckTest
        {

        [Fact]
        public async Task AddItemToCart_ReturnsServerError_WhenServiceFails()
            {
            // Arrange
            var application = new CartServiceWebApplicationFactoryWithFailingRepository();

            var client = application.CreateClient();

            var newCartItem = new CartItemDTO
                {
                userId = 1,
                ProductId = 102,
                Quantity = 3,
                itemTotalPrice = 75.00m
                };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/cart/add", newCartItem);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Contains("An error occurred while adding the item to the cart.", errorMessage);
            }
        [Fact]
        public async Task UpdateCartItemQuantity_ReturnsServerError_WhenServiceFails()
            {
            // Arrange
            var application = new CartServiceWebApplicationFactoryWithFailingRepository();
            var client = application.CreateClient();

            var updates = new List<UpdateQuantityDTO>
        {
            new UpdateQuantityDTO
            {
            CartItemId = 999,
            NewQuantity = 2,
            newTotalPrice = 50.00m
            }
                 };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/cart/update-quantity", updates);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            var error = await response.Content.ReadAsStringAsync();
            Assert.Contains("An error occurred while updating item quantities", error);
            }


        }
    }