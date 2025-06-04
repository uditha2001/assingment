using CartService.API.DTO;
using CartService.API.Model.Entities;
using CartService.API.repository.interfaces;
using CartService.API.services;
using CartService.API.services.interfaces;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;

namespace CartService.Tests.Unit.Services
{
    public class CartServiceTest
    {
        private readonly CartServiceIMPL _cartServiceimpl;
        private readonly Mock<ICartRepository> _cartRepositoryMock;

        public CartServiceTest()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            var mockServiceUrls = new ServiceUrls
            {
                OrderService = "http://localhost"
            };

            var options = Options.Create(mockServiceUrls);
            _cartServiceimpl = new CartServiceIMPL(_cartRepositoryMock.Object, new HttpClient(), options);
        }

        private CartItemEntity CreateSampleEntity()
        {
            return new CartItemEntity
            {
                cartItemId = 1,
                itemTotalPrice = 30,
                Quantity = 8,
                ProductId = 1,
                UserId=1
            };

        }

        private HttpClient CreateMockHttpClient(HttpStatusCode statusCode)
        {
            var messageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode
                })
                .Verifiable();

            return new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
        }


        [Fact]
        public async Task AddItemToCartAsync_ReturnsTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            var cartItem = new CartItemDTO {itemTotalPrice=48,cartItemId=1,ProductId=1,Quantity=8,userId=1};
            _cartRepositoryMock.Setup(r => r.AddOrUpdateCartItemAsync(It.IsAny<CartItemEntity>()))
                .ReturnsAsync(true);

            // Act
            var result = await _cartServiceimpl.AddItemToCartAsync(cartItem);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task AddItemToCartAsync_ReturnsFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            var cartItem = new CartItemDTO { itemTotalPrice = 48, cartItemId = 1, ProductId = 1, Quantity = 8, userId = 1 };
            _cartRepositoryMock.Setup(r => r.AddOrUpdateCartItemAsync(It.IsAny<CartItemEntity>()))
                .ReturnsAsync(false);

            // Act
            var result = await _cartServiceimpl.AddItemToCartAsync(cartItem);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddItemToCartAsync_ThrowsApplicationException_WhenRepositoryThrows()
        {
            // Arrange
            var cartItem = new CartItemDTO { itemTotalPrice = 48, cartItemId = 1, ProductId = 1, Quantity = 8, userId = 1 };
            _cartRepositoryMock.Setup(r => r.AddOrUpdateCartItemAsync(It.IsAny<CartItemEntity>()))
                .ThrowsAsync(new Exception("DB failure"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _cartServiceimpl.AddItemToCartAsync(cartItem));
            Assert.Contains("Failed to add or update item in cart", ex.Message);
            Assert.NotNull(ex.InnerException);
        }
        [Fact]
        public async Task ClearCartAsync_ReturnsTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            long userId = 1;
            _cartRepositoryMock.Setup(r => r.ClearCartAsync(userId))
                .ReturnsAsync(true);
            // Act
            var result = await _cartServiceimpl.ClearCartAsync(userId);
            // Assert
            Assert.True(result);
        }
        [Fact]
        public async Task ClearCartAsync_ThrowsApplicationException_WhenRepositoryThrowsException()
        {
            // Arrange
            long userId = 1;
            _cartRepositoryMock.Setup(repo => repo.ClearCartAsync(userId))
                               .ThrowsAsync(new Exception("DB error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(
                () => _cartServiceimpl.ClearCartAsync(userId));

            Assert.Contains("Failed to clear cart", exception.Message);
            Assert.IsType<Exception>(exception.InnerException);
        }

        [Fact]
        public async Task ClearCartAsync_ReturnsFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            long userId = 1;
            _cartRepositoryMock.Setup(r => r.ClearCartAsync(userId))
                .ReturnsAsync(false);
            // Act
            var result = await _cartServiceimpl.ClearCartAsync(userId);
            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetCartByUserIdAsync_ReturnsListOfDTOs_WhenItemsExist()
        {
            // Arrange
            long userId = 1;
            var entities = new List<CartItemEntity> { CreateSampleEntity() };
            _cartRepositoryMock.Setup(r => r.GetCartByUserIdAsync(userId))
                               .ReturnsAsync(entities);

            // Act
            var result = await _cartServiceimpl.GetCartByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, item => Assert.IsType<CartItemDTO>(item));
            Assert.Equal(entities.Count, result.Count);
        }
        [Fact]
        public async Task GetCartByUserIdAsync_ReturnsEmptyList_WhenCartIsEmpty()
        {
            // Arrange
            long userId = 1;
            var emptyEntities = new List<CartItemEntity>(); 
            _cartRepositoryMock.Setup(r => r.GetCartByUserIdAsync(userId))
                               .ReturnsAsync(emptyEntities);

            // Act
            var result = await _cartServiceimpl.GetCartByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCartByUserIdAsync_ShouldThrowApplicationException_WhenRepositoryFails()
        {
            //Arrange
            var userId = 1;
            _cartRepositoryMock.Setup(repo => repo.GetCartByUserIdAsync(userId)).ThrowsAsync(new Exception("DB Error"));
            //Act
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _cartServiceimpl.GetCartByUserIdAsync(userId));
            //Assert
            Assert.Equal("Failed to retrieve cart items.", ex.Message);
            Assert.IsType<Exception>(ex.InnerException);
        }
        [Fact]
        public async Task RemoveItemFromCartAsync_ShouldReturnTrue_WhenRemovalIsSuccessful()
        {
            // Arrange
            long cartItemId = 1;
            _cartRepositoryMock.Setup(r => r.RemoveItemFromCartAsync(cartItemId)).ReturnsAsync(true);

            // Act
            var result = await _cartServiceimpl.RemoveItemFromCartAsync(cartItemId);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public async Task RemoveItemFromCartAsync_ShouldReturnFalse_WhenRemovalFails()
        {
            // Arrange
            long cartItemId = 2;
            _cartRepositoryMock.Setup(r => r.RemoveItemFromCartAsync(cartItemId)).ReturnsAsync(false);

            // Act
            var result = await _cartServiceimpl.RemoveItemFromCartAsync(cartItemId);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public async Task RemoveItemFromCartAsync_ShouldThrowApplicationException_WhenRepositoryThrows()
        {
            // Arrange
            long cartItemId = 3;
            _cartRepositoryMock.Setup(r => r.RemoveItemFromCartAsync(cartItemId)).ThrowsAsync(new Exception("DB error"));

            // Act 
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _cartServiceimpl.RemoveItemFromCartAsync(cartItemId));
            //Assert
            Assert.Equal("Failed to remove item from cart.", ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal("DB error", ex.InnerException.Message);
        }
        [Fact]
        public async Task UpdateItemQuantityAsync_ShouldReturnTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            long cartItemId = 1;
            int newQuantity = 2;
            decimal newTotalPrice = 99.99m;

            _cartRepositoryMock
                .Setup(r => r.UpdateItemQuantityAsync(cartItemId, newQuantity, newTotalPrice))
                .ReturnsAsync(true);

            // Act
            var result = await _cartServiceimpl.UpdateItemQuantityAsync(cartItemId, newQuantity, newTotalPrice);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public async Task UpdateItemQuantityAsync_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            long cartItemId = 2;
            int newQuantity = 5;
            decimal newTotalPrice = 250.00m;

            _cartRepositoryMock
                .Setup(r => r.UpdateItemQuantityAsync(cartItemId, newQuantity, newTotalPrice))
                .ReturnsAsync(false);

            // Act
            var result = await _cartServiceimpl.UpdateItemQuantityAsync(cartItemId, newQuantity, newTotalPrice);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public async Task UpdateItemQuantityAsync_ShouldThrowApplicationException_WhenRepositoryThrows()
        {
            // Arrange
            long cartItemId = 3;
            int newQuantity = 1;
            decimal newTotalPrice = 49.99m;

            _cartRepositoryMock
                .Setup(r => r.UpdateItemQuantityAsync(cartItemId, newQuantity, newTotalPrice))
                .ThrowsAsync(new Exception("DB error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                _cartServiceimpl.UpdateItemQuantityAsync(cartItemId, newQuantity, newTotalPrice));

            Assert.Equal("Failed to update item quantity.", ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal("DB error", ex.InnerException.Message);
        }
        [Fact]
        public async Task SubmitOrderAsync_ShouldReturnTrue_WhenOrderIsSuccessfulAndCartIsCleared()
        {
            // Arrange
            long userId = 1;
            var cartItems = new List<CartItemEntity>
    {
        new CartItemEntity { ProductId = 1, Quantity = 2, itemTotalPrice = 100m }
    };

            var mockHttpClient = CreateMockHttpClient(HttpStatusCode.OK);

            _cartRepositoryMock.Setup(r => r.GetCartByUserIdAsync(userId)).ReturnsAsync(cartItems);
            _cartRepositoryMock.Setup(r => r.ClearCartAsync(userId)).ReturnsAsync(true);
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK);
            var serviceUrls = Options.Create(new ServiceUrls
            {
                OrderService = "http://localhost"
            });
            var cartService = new CartServiceIMPL(_cartRepositoryMock.Object, httpClient, serviceUrls);

            // Act
            var result = await cartService.SubmitOrderAsync(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SubmitOrderAsync_ThrowsApplicationException_WithInvalidOperationExceptionInner_WhenCartIsEmpty()
        {
            // Arrange
            long userId = 1;
            _cartRepositoryMock.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync(new List<CartItemEntity>());

            var httpClient = CreateMockHttpClient(HttpStatusCode.OK);
            var serviceUrls = Options.Create(new ServiceUrls
            {
                OrderService = "http://localhost"
            });
            var cartService = new CartServiceIMPL(_cartRepositoryMock.Object, httpClient, serviceUrls);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => cartService.SubmitOrderAsync(userId));

            Assert.NotNull(ex.InnerException);
            Assert.IsType<InvalidOperationException>(ex.InnerException);
            Assert.Equal("No items in cart to submit.", ex.InnerException.Message);
        }


        [Fact]
        public async Task SubmitOrderAsync_ThrowsApplicationException_WithHttpRequestExceptionInner_WhenOrderServiceFails()
        {
            // Arrange
            long userId = 1;
            var cartItems = new List<CartItemEntity>
    {
        new CartItemEntity { ProductId = 1, Quantity = 2, itemTotalPrice = 100 }
    };

            var httpClient = CreateMockHttpClient(HttpStatusCode.InternalServerError);
            _cartRepositoryMock.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync(cartItems);
            var serviceUrls = Options.Create(new ServiceUrls
            {
                OrderService = "http://localhost"
            });
            var cartService = new CartServiceIMPL(_cartRepositoryMock.Object, httpClient, serviceUrls);


            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => cartService.SubmitOrderAsync(userId));

            Assert.NotNull(ex.InnerException);
            Assert.IsType<HttpRequestException>(ex.InnerException);
            Assert.Contains("Order submission failed", ex.InnerException.Message);
            Assert.Contains("StatusCode: InternalServerError", ex.InnerException.Message);
        }


        [Fact]
        public async Task SubmitOrderAsync_ReturnsFalse_WhenCartClearFails()
        {
            // Arrange
            long userId = 1;
            var cartItems = new List<CartItemEntity>
    {
        new CartItemEntity { ProductId = 1, Quantity = 2, itemTotalPrice = 100 }
    };

            var httpClient = CreateMockHttpClient(HttpStatusCode.OK);
            _cartRepositoryMock.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync(cartItems);
            _cartRepositoryMock.Setup(x => x.ClearCartAsync(userId)).ReturnsAsync(false);

            var serviceUrls = Options.Create(new ServiceUrls
            {
                OrderService = "http://localhost"
            });
            var cartService = new CartServiceIMPL(_cartRepositoryMock.Object, httpClient, serviceUrls);

            // Act
            var result = await cartService.SubmitOrderAsync(userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetCartItemsCount_ShouldReturnCount_WhenRepositorySucceeds()
        {
            // Arrange
            long userId = 1;
            int expectedCount = 5;

            _cartRepositoryMock
                .Setup(repo => repo.GetCartItemsCount(userId))
                .ReturnsAsync(expectedCount);

            // Act
            var result = await _cartServiceimpl.GetCartItemsCount(userId);

            // Assert
            Assert.Equal(expectedCount, result);
        }
        [Fact]
        public async Task GetCartItemsCount_ShouldThrowApplicationException_WhenRepositoryFails()
        {
            // Arrange
            long userId = 1;

            _cartRepositoryMock
                .Setup(repo => repo.GetCartItemsCount(userId))
                .ThrowsAsync(new Exception("DB failure"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                _cartServiceimpl.GetCartItemsCount(userId));

            Assert.Equal("failed to get count", ex.Message);
        }






    }
}
