using Microsoft.Extensions.Logging;
using Moq;
using OrderService.API.DTO;
using OrderService.API.Models.Entity;
using OrderService.API.Repository.Interface;
using OrderService.API.services;

namespace OrderServiceTest.Services
{
    public class OrderServiceIMPLTest
    {
        private readonly OrderServiceIMPL _orderService;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ILogger<OrderServiceIMPL>> _loggerMock;

        public OrderServiceIMPLTest()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _loggerMock = new Mock<ILogger<OrderServiceIMPL>>();
            _orderService = new OrderServiceIMPL(_orderRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldReturnNumberLargerThanZero_WhenSuccessful()
        {
            // Arrange
            var orderItemsDTOs = new List<OrderItemsDTO>
            {
                new OrderItemsDTO
                {
                    orderId = 1,
                    itemTotalPrice = 33,
                    ProductId = 1,
                    orderItemId = 1,
                    quantity = 8
                }
            };

            var orderDto = new OrderDTO
            {
                orderId = 1,
                totalOrderprice = 48,
                userId = 1,
                items = orderItemsDTOs
            };

            _orderRepositoryMock
                .Setup(repo => repo.CreateOrderAsync(It.IsAny<OrderEntity>()))
                .ReturnsAsync(123);

            // Act
            var result = await _orderService.CreateOrderAsync(orderDto);

            // Assert
            Assert.True(result > 0);
            Assert.Equal(123, result);
            _orderRepositoryMock.Verify(
                r => r.CreateOrderAsync(It.IsAny<OrderEntity>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldLogErrorAndThrow_WhenRepositoryThrows()
        {
            // Arrange
            var orderDto = new OrderDTO
            {
                orderId = 1,
                totalOrderprice = 100,
                userId = 1,
                items = new List<OrderItemsDTO>
                {
                    new OrderItemsDTO
                    {
                        orderId = 1,
                        itemTotalPrice = 100,
                        ProductId = 1,
                        orderItemId = 1,
                        quantity = 2
                    }
                }
            };

            var exception = new Exception("DB failed");

            _orderRepositoryMock
                .Setup(repo => repo.CreateOrderAsync(It.IsAny<OrderEntity>()))
                .ThrowsAsync(exception);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _orderService.CreateOrderAsync(orderDto));

            Assert.Equal("DB failed", ex.Message);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
        [Fact]
        public async Task DeleteOrderAsync_ShouldReturnTrue_WhenDeletionSucceeds()
        {
            // Arrange
            int orderId = 1;
            _orderRepositoryMock
                .Setup(repo => repo.DeleteOrderAsync(orderId))
                .ReturnsAsync(true);

            // Act
            var result = await _orderService.DeleteOrderAsync(orderId);

            // Assert
            Assert.True(result);
            _orderRepositoryMock.Verify(r => r.DeleteOrderAsync(orderId), Times.Once);
        }
        [Fact]
        public async Task DeleteOrderAsync_ShouldLogErrorAndThrow_WhenRepositoryThrows()
        {
            // Arrange
            int orderId = 1;
            var exception = new Exception("DB error");

            _orderRepositoryMock
                .Setup(repo => repo.DeleteOrderAsync(orderId))
                .ThrowsAsync(exception);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _orderService.DeleteOrderAsync(orderId));
            Assert.Equal("DB error", ex.Message);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnOrderDTOList_WhenRepositoryReturnsEntities()
        {
            // Arrange
            long userId = 1;

            var orderEntities = new List<OrderEntity>
    {
        new OrderEntity
        {
            orderId = 1,
            totalOrderprice = 99.99m,
            userId = 1,
            items = new List<OrderItemEntity>
            {
                new OrderItemEntity
                {
                    orderItemId = 1,
                    orderId = 1,
                    ProductId = 10,
                    quantity = 2,
                    itemTotalPrice = 49.99m
                }
            }
        }
    };

            _orderRepositoryMock
                .Setup(repo => repo.GetOrdersByUserIdAsync(userId))
                .ReturnsAsync(orderEntities);

            // Act
            var result = await _orderService.GetAllOrdersAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result[0].orderId);
            Assert.Equal(99.99m, result[0].totalOrderprice);
            Assert.Equal(1, result[0].userId);
            _orderRepositoryMock.Verify(r => r.GetOrdersByUserIdAsync(userId), Times.Once);
        }
        [Fact]
        public async Task GetAllOrdersAsync_ShouldLogErrorAndThrow_WhenRepositoryThrows()
        {
            // Arrange
            long userId = 1;
            var exception = new Exception("DB Error");

            _orderRepositoryMock
                .Setup(repo => repo.GetOrdersByUserIdAsync(userId))
                .ThrowsAsync(exception);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _orderService.GetAllOrdersAsync(userId));
            Assert.Equal("DB Error", ex.Message);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrderDTO_WhenOrderExists()
        {
            // Arrange
            int orderId = 1;

            var orderEntity = new OrderEntity
            {
                orderId = orderId,
                userId = 1,
                totalOrderprice = 99.99m,
                items = new List<OrderItemEntity>
        {
            new OrderItemEntity
            {
                orderItemId = 1,
                orderId = orderId,
                ProductId = 101,
                quantity = 2,
                itemTotalPrice = 49.99m
            }
        }
            };

            _orderRepositoryMock
                .Setup(repo => repo.GetOrderByIdAsync(orderId))
                .ReturnsAsync(orderEntity);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.orderId);
            Assert.Equal(99.99m, result.totalOrderprice);
            Assert.Equal(1, result.userId);
            Assert.Single(result.items);
            Assert.Equal(49.99m, result.items[0].itemTotalPrice);

            _orderRepositoryMock.Verify(r => r.GetOrderByIdAsync(orderId), Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldLogErrorAndThrow_WhenRepositoryThrows()
        {
            // Arrange
            int orderId = 1;
            var exception = new Exception("DB failure");

            _orderRepositoryMock
                .Setup(repo => repo.GetOrderByIdAsync(orderId))
                .ThrowsAsync(exception);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _orderService.GetOrderByIdAsync(orderId));
            Assert.Equal("DB failure", ex.Message);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
        [Fact]
        public async Task UpdateOrderAsync_ShouldReturnTrue_WhenUpdateIsSuccessful()
        {
            // Arrange
            var orderDto = new OrderDTO
            {
                orderId = 1,
                userId = 1,
                totalOrderprice = 100.00m,
                items = new List<OrderItemsDTO>
        {
            new OrderItemsDTO
            {
                orderItemId = 1,
                orderId = 1,
                ProductId = 101,
                quantity = 3,
                itemTotalPrice = 33.33m
            }
        }
            };

            _orderRepositoryMock
                .Setup(repo => repo.UpdateOrderAsync(It.IsAny<OrderEntity>()))
                .ReturnsAsync(true);

            // Act
            var result = await _orderService.UpdateOrderAsync(orderDto);

            // Assert
            Assert.True(result);
            _orderRepositoryMock.Verify(r => r.UpdateOrderAsync(It.IsAny<OrderEntity>()), Times.Once);
        }
        [Fact]
        public async Task UpdateOrderAsync_ShouldLogErrorAndThrow_WhenRepositoryThrows()
        {
            // Arrange
            var orderDto = new OrderDTO
            {
                orderId = 1,
                userId = 1,
                totalOrderprice = 100.00m,
                items = new List<OrderItemsDTO>
        {
            new OrderItemsDTO
            {
                orderItemId = 1,
                orderId = 1,
                ProductId = 101,
                quantity = 3,
                itemTotalPrice = 33.33m
            }
        }
            };

            var exception = new Exception("DB failed");

            _orderRepositoryMock
                .Setup(repo => repo.UpdateOrderAsync(It.IsAny<OrderEntity>()))
                .ThrowsAsync(exception);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _orderService.UpdateOrderAsync(orderDto));
            Assert.Equal("DB failed", ex.Message);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }






    }
}
