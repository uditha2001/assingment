using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using OrderService.API.DTO;
using ProductService.API.DTO;
using ProductService.API.Models.Entities;
using ProductService.API.Repository.RepositoryInterfaces;
using ProductService.API.Services;
using ProductService.API.Services.serviceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ProjectServiceTest.Services
{
    public class ProductServiceIMPLTest
        {
        private readonly ProductServiceImpl _productServiceIMPL;
        private readonly Mock<IProductRepo> _productRepositoryMock;
        private readonly Mock<ILogger<ProductServiceImpl>> _loggerMock;
        private readonly Mock<IOptions<ServiceUrls>> _optionsMock;
        private readonly Mock<IProductContentService> _productContentServiceMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _mockHttpClient;

        public ProductServiceIMPLTest()
            {
            _productRepositoryMock = new Mock<IProductRepo>();
            _loggerMock = new Mock<ILogger<ProductServiceImpl>>();
            _productContentServiceMock = new Mock<IProductContentService>();
            var serviceUrls = new ServiceUrls
                {
                AdapterFactoryService = "http://localhost",
                RedirectUrl = "http://localhost"
                };
            _optionsMock = new Mock<IOptions<ServiceUrls>>();
            _optionsMock.Setup(o => o.Value).Returns(serviceUrls);
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _mockHttpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _productServiceIMPL = new ProductServiceImpl(_productRepositoryMock.Object, _loggerMock.Object, _optionsMock.Object, _mockHttpClient, _productContentServiceMock.Object);
            }


        private List<ProductDTO> GetSampleProductDTOList()
            {
            return new List<ProductDTO>()
        {
            new ProductDTO
            {
                Id = 1,
                Name = "Product 1",
                Description = "Description 1",
                Price = 10,
                Currency = "USD",
                originId = 100,
                provider = "Provider1",
                availableQuantity = 5,
                owner = "owner1",
                ProductCategoryId = 1,
                Attributes = new List<ProductAttributesDTO>(),
                Contents = new List<ProductContentDTO>()
            }
        };
            }

        [Fact]
        public async Task UpdateProductsFromAdapterAsync_ReturnsNull_WhenHttpResponseIsUnsuccessful()
            {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                    {
                    StatusCode = HttpStatusCode.BadRequest
                    });

            // Act
            var result = await _productServiceIMPL.UpdateProductsFromAdapterAsync();

            // Assert
            Assert.Null(result);
            }

        [Fact]
        public async Task UpdateProductsFromAdapterAsync_ReturnsEmptyList_WhenResponseContentIsEmpty()
            {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                    {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(new List<ProductDTO>())
                    });

            // Act
            var result = await _productServiceIMPL.UpdateProductsFromAdapterAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            }

        [Fact]
        public async Task UpdateProductsFromAdapterAsync_UpdatesExistingProduct_WhenProductExists()
            {
            // Arrange
            var productsList = GetSampleProductDTOList();

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                    {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(productsList)
                    });

            _productRepositoryMock
                .Setup(repo => repo.GetExternalProductsWithOriginIdAsync(It.IsAny<ProductDTO>()))
                .ReturnsAsync(new ProductEntity
                    {
                    Id = 1,
                    Name = "Old Product",
                    originId = 100,
                    Provider = "Provider1"
                    });

            _productRepositoryMock.Setup(repo => repo.RemoveAllProductAttributesByProvider(It.IsAny<ProductEntity>())).Returns(Task.CompletedTask);
            _productRepositoryMock.Setup(repo => repo.RemoveAllProductContentsWhereProviderNotEmpty(It.IsAny<ProductEntity>())).Returns(Task.CompletedTask);
            _productRepositoryMock.Setup(repo => repo.UpdateProductAsync(It.IsAny<ProductEntity>())).Returns(Task.FromResult(2));

            // You may want to mock ExtractAttributesAndContentFromProductDTO if it is virtual or use a partial mock.

            // Act
            var result = await _productServiceIMPL.UpdateProductsFromAdapterAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);

            _productRepositoryMock.Verify(repo => repo.GetExternalProductsWithOriginIdAsync(It.IsAny<ProductDTO>()), Times.Once);
            _productRepositoryMock.Verify(repo => repo.RemoveAllProductAttributesByProvider(It.IsAny<ProductEntity>()), Times.Once);
            _productRepositoryMock.Verify(repo => repo.RemoveAllProductContentsWhereProviderNotEmpty(It.IsAny<ProductEntity>()), Times.Once);
            _productRepositoryMock.Verify(repo => repo.UpdateProductAsync(It.IsAny<ProductEntity>()), Times.Once);
            _productRepositoryMock.Verify(repo => repo.AddProduct(It.IsAny<ProductEntity>()), Times.Never);
            }

        [Fact]
        public async Task UpdateProductsFromAdapterAsync_AddsNewProduct_WhenProductDoesNotExist()
            {
            // Arrange
            var productsList = GetSampleProductDTOList();

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                    {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(productsList)
                    });

            _productRepositoryMock
                .Setup(repo => repo.GetExternalProductsWithOriginIdAsync(It.IsAny<ProductDTO>()))
                .ReturnsAsync((ProductEntity?)null);

            _productRepositoryMock.Setup(repo => repo.AddProduct(It.IsAny<ProductEntity>())).Returns(Task.FromResult(2));

            // Act
            var result = await _productServiceIMPL.UpdateProductsFromAdapterAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);

            _productRepositoryMock.Verify(repo => repo.GetExternalProductsWithOriginIdAsync(It.IsAny<ProductDTO>()), Times.Once);
            _productRepositoryMock.Verify(repo => repo.AddProduct(It.IsAny<ProductEntity>()), Times.Once);
            _productRepositoryMock.Verify(repo => repo.UpdateProductAsync(It.IsAny<ProductEntity>()), Times.Never);
            }

        [Fact]
        public async Task UpdateProductsFromAdapterAsync_ReturnsNull_OnException()
            {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception("Network failure"));

            // Act
            var result = await _productServiceIMPL.UpdateProductsFromAdapterAsync();

            // Assert
            Assert.Null(result);
            }


        [Fact]
        public async Task GetAllProducts_ReturnsListOfProductDTO()
            {
            // Arrange
            var productEntities = new List<ProductEntity>
        {
            new ProductEntity { Id = 1, Name = "Test Product" }
        };

            _productRepositoryMock.Setup(repo => repo.GetAllProducts())
                .ReturnsAsync(productEntities);

            // Act
            var result = await _productServiceIMPL.GetAllProducts();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Product", result[0].Name);
            }

        [Fact]
        public async Task GetAllProducts_ThrowsExceptionAndLogsError()
            {
            // Arrange
            _productRepositoryMock.Setup(repo => repo.GetAllProducts())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _productServiceIMPL.GetAllProducts());
            Assert.Equal("failed to get products", ex.Message);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
            }

        [Fact]
        public async Task CreateProduct_ReturnsProductId_WhenSuccess()
            {
            // Arrange
            var productDto = new ProductDTO
                {
                Id = 0,
                Name = "New Product"
                // Populate additional fields as needed
                };

            var expectedId = 123L;
            _productRepositoryMock
                .Setup(repo => repo.SaveProduct(It.IsAny<ProductEntity>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _productServiceIMPL.CreateProduct(productDto);

            // Assert
            Assert.Equal(expectedId, result);
            }

        [Fact]
        public async Task CreateProduct_ThrowsException_WhenRepoFails()
            {
            // Arrange
            var productDto = new ProductDTO { Name = "Fail Product" };

            _productRepositoryMock
                .Setup(repo => repo.SaveProduct(It.IsAny<ProductEntity>()))
                .ThrowsAsync(new Exception("DB save failed"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _productServiceIMPL.CreateProduct(productDto));
            Assert.Equal("fiailed create product", ex.Message);
            }

        [Fact]
        public async Task DeleteProductAsync_ProductAndContentsDeleted_ReturnsTrue()
            {
            // Arrange
            var productId = 1L;

            var productEntity = new ProductEntity
                {
                Id = productId,
                Contents = new List<ProductContentEntity>
                 {
                       new ProductContentEntity { ContentId = 100 },
                       new ProductContentEntity { ContentId = 101 }
                 }
                };

            _productRepositoryMock
                .Setup(repo => repo.GetProductById(productId))
                .ReturnsAsync(productEntity);

            _productContentServiceMock
                .Setup(service => service.DeleteContent(It.IsAny<long>()))
                .ReturnsAsync(true);

            _productRepositoryMock
                .Setup(repo => repo.DeleteProductAsync(productId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productServiceIMPL.DeleteProductAsync(productId);

            // Assert
            Assert.True(result);

            _productContentServiceMock.Verify(service => service.DeleteContent(100), Times.Once);
            _productContentServiceMock.Verify(service => service.DeleteContent(101), Times.Once);
            _productRepositoryMock.Verify(repo => repo.DeleteProductAsync(productId), Times.Once);
            }

        [Fact]
        public async Task DeleteProductAsync_ContentDeletionFails_LogsWarningButReturnsTrue()
            {
            // Arrange
            var productId = 1L;

            var productEntity = new ProductEntity
                {
                Id = productId,
                Contents = new List<ProductContentEntity>
                            {
                              new ProductContentEntity { ContentId = 100 },
                              new ProductContentEntity { ContentId = 101 }
                            }
                };

            _productRepositoryMock
                .Setup(repo => repo.GetProductById(productId))
                .ReturnsAsync(productEntity);

            _productContentServiceMock
                .SetupSequence(service => service.DeleteContent(It.IsAny<long>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true);

            _productRepositoryMock
                .Setup(repo => repo.DeleteProductAsync(productId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productServiceIMPL.DeleteProductAsync(productId);

            // Assert
            Assert.True(result);

            _productContentServiceMock.Verify(service => service.DeleteContent(100), Times.Once);
            _productContentServiceMock.Verify(service => service.DeleteContent(101), Times.Once);
            _productRepositoryMock.Verify(repo => repo.DeleteProductAsync(productId), Times.Once);

            }

        [Fact]
        public async Task DeleteProductAsync_RepoThrowsException_ReturnsFalse()
            {
            // Arrange
            var productId = 1L;

            _productRepositoryMock
                .Setup(repo => repo.GetProductById(productId))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _productServiceIMPL.DeleteProductAsync(productId);


            // Assert
            Assert.False(result);
            }

        [Fact]
        public async Task GetInternalSystemProducts_ReturnsProductDTOList()
            {
            // Arrange
            var productEntities = new List<ProductEntity>
                      {
                         new ProductEntity { Id = 1, Name = "Product 1" },
                         new ProductEntity { Id = 2, Name = "Product 2" }
                      };

            _productRepositoryMock
                .Setup(repo => repo.GetInternalSystemProducts())
                .ReturnsAsync(productEntities);

            // Act
            var result = await _productServiceIMPL.GetInternalSystemProducts();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productEntities.Count, result.Count);
            Assert.All(result, item => Assert.IsType<ProductDTO>(item));
            }

        [Fact]
        public async Task GetInternalSystemProducts_WhenException_ReturnsEmptyList()
            {
            // Arrange
            _productRepositoryMock
                .Setup(repo => repo.GetInternalSystemProducts())
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _productServiceIMPL.GetInternalSystemProducts();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            }

        [Fact]
        public async Task SellProducts_InternalProduct_SufficientQuantity_ReturnsTrue()
            {
            // Arrange
            var orderList = new List<CheckoutDTO>
            {
                  new CheckoutDTO { ProductId = 1, quantity = 5 }
            };

            var productEntity = new ProductEntity { Id = 1, availableQuantity = 10 };

            _productRepositoryMock.Setup(r => r.GetProductById(1)).ReturnsAsync(productEntity);
            _productRepositoryMock.Setup(r => r.CheckInternalSystemProduct(1)).ReturnsAsync(true);
            _productRepositoryMock.Setup(r => r.SellProducts(1, 5)).Returns(Task.FromResult(true));

            // Act
            var result = await _productServiceIMPL.SellProducts(orderList);

            // Assert
            Assert.True(result);
            _productRepositoryMock.Verify(r => r.SellProducts(1, 5), Times.Once);
            }

        [Fact]
        public async Task SellProducts_ExternalProduct_HttpSuccessWithTrue_ReturnsTrue()
            {
            // Arrange
            var orderList = new List<CheckoutDTO>
         {
        new CheckoutDTO { ProductId = 2, quantity = 5 }
        };

            var productEntity = new ProductEntity { Id = 2, availableQuantity = 10 };

            _productRepositoryMock.Setup(r => r.GetProductById(2)).ReturnsAsync(productEntity);
            _productRepositoryMock.Setup(r => r.CheckInternalSystemProduct(2)).ReturnsAsync(false);

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
                {
                Content = new StringContent("true")
                };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post
                        && req.RequestUri == new Uri($"{_optionsMock.Object.Value.AdapterFactoryService}/api/v1/Adapter")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _productServiceIMPL.SellProducts(orderList);

            // Assert
            Assert.True(result);
            }


        [Fact]
        public async Task SellProducts_ExternalProduct_HttpResponseFails_ReturnsFalse()
            {
            // Arrange
            var orderList = new List<CheckoutDTO>
             {
                 new CheckoutDTO { ProductId = 3, quantity = 5 }
             };

            var productEntity = new ProductEntity { Id = 3, availableQuantity = 10 };

            _productRepositoryMock.Setup(r => r.GetProductById(3)).ReturnsAsync(productEntity);
            _productRepositoryMock.Setup(r => r.CheckInternalSystemProduct(3)).ReturnsAsync(false);

            var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post
                        && req.RequestUri == new Uri($"{_optionsMock.Object.Value.AdapterFactoryService}/api/v1/Adapter")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _productServiceIMPL.SellProducts(orderList);

            // Assert
            Assert.False(result);
            }

        [Fact]
        public async Task SellProducts_WhenException_ThrowsExceptionAndLogsError()
            {
            // Arrange
            var orderList = new List<CheckoutDTO>
            {
                 new CheckoutDTO { ProductId = 4, quantity = 5 }
            };

            _productRepositoryMock
                .Setup(r => r.GetProductById(It.IsAny<long>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _productServiceIMPL.SellProducts(orderList));

            // Verify logger called with LogLevel.Error and a message containing "An error occurred"
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            }

        [Fact]
        public async Task GetExtranalProductById_ShouldReturnProductDTO()
            {
            // Arrange
            var productId = 1L;
            var productEntity = new ProductEntity
                {
                Id = productId,
                Name = "Test Product",
                availableQuantity = 10
                // Add other properties if needed
                };

            _productRepositoryMock
                .Setup(repo => repo.GetExternalProductByIdAsync(productId))
                .ReturnsAsync(productEntity);

            // Act
            var result = await _productServiceIMPL.GetExtranalProductById(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("Test Product", result.Name);
            }

        [Fact]
        public async Task GetExtranalProductById_WhenException_ThrowsAndLogsError()
            {
            // Arrange
            var productId = 1L;
            _productRepositoryMock
                .Setup(repo => repo.GetExternalProductByIdAsync(productId))
                .ThrowsAsync(new Exception("DB failure"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _productServiceIMPL.GetExtranalProductById(productId));

            // Verify logging
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            }
        [Fact]
        public async Task GetProductById_ReturnsProductDTO()
            {
            // Arrange
            var productId = 1;
            var productEntity = new ProductEntity
                {
                Id = productId,
                Name = "Test Product",
                // ... add other required fields
                };

            _productRepositoryMock
                .Setup(r => r.GetProductById(productId))
                .ReturnsAsync(productEntity);

            // Act
            var result = await _productServiceIMPL.GetProductById(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("Test Product", result.Name);
            }

        [Fact]
        public async Task GetProductById_WhenExceptionThrown_LogsErrorAndThrows()
            {
            // Arrange
            var productId = 99;
            var exception = new Exception("DB failure");

            _productRepositoryMock
                .Setup(r => r.GetProductById(productId))
                .ThrowsAsync(exception);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _productServiceIMPL.GetProductById(productId));
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
        public async Task GetAllCategories_ReturnsCategoryDTOList()
            {
            List<ProductEntity> productEntities = new List<ProductEntity>();
            ProductEntity testproduct = new ProductEntity
                {
                Id = 1,
                Name = "Test Product",
                Description = "test product1",
                Price = 100,
                Currency = "USD",
                availableQuantity = 10,
                owner = "owner1",
                ProductCategoryId = 1,
                Attributes = new List<ProductAttributesEntity>(),
                Contents = new List<ProductContentEntity>()
                };
            productEntities.Add(testproduct);
            // Arrange
            var categoryEntities = new List<ProductCategoryEntity>
            {
                new ProductCategoryEntity { Id = 1, Name = "Electronics",Description="test product1",Product=productEntities },
                new ProductCategoryEntity { Id = 2, Name = "Books" }
            };

            _productRepositoryMock
                .Setup(repo => repo.GetAllCategories())
                .ReturnsAsync(categoryEntities);

            // Act
            var result = await _productServiceIMPL.GetAllCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Electronics");
            Assert.Contains(result, c => c.Name == "Books");
            }
        [Fact]
        public async Task GetAllCategories_WhenExceptionThrown_LogsErrorAndThrows()
            {
            // Arrange
            var exception = new Exception("DB error");

            _productRepositoryMock
                .Setup(repo => repo.GetAllCategories())
                .ThrowsAsync(exception);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _productServiceIMPL.GetAllCategories());
            Assert.Equal("failed to get categories", ex.Message);

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
        public async Task GetOwnerProducts_ReturnsProductDTOList()
            {
            // Arrange
            long userId = 1;

            var productEntities = new List<ProductEntity>
            {
              new ProductEntity { Id = 101, Name = "Laptop", availableQuantity = 10 },
              new ProductEntity { Id = 102, Name = "Phone", availableQuantity = 5 }
            };

            _productRepositoryMock
                .Setup(repo => repo.GetOwnerProducts(userId))
                .ReturnsAsync(productEntities);

            // Act
            var result = await _productServiceIMPL.GetOwnerProducts(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Name == "Laptop");
            Assert.Contains(result, p => p.Name == "Phone");
            }

        [Fact]
        public async Task GetOwnerProducts_WhenExceptionThrown_LogsErrorAndThrows()
            {
            // Arrange
            long userId = 99;
            var exception = new Exception("Database connection failed");

            _productRepositoryMock
                .Setup(repo => repo.GetOwnerProducts(userId))
                .ThrowsAsync(exception);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _productServiceIMPL.GetOwnerProducts(userId));
            Assert.Equal("failed to get products", ex.Message);

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
        public async Task GetCheckout_InternalProduct_WithSufficientQuantity_ReturnsTrue()
            {
            // Arrange
            var order = new CheckoutDTO { ProductId = 1, quantity = 2, itemTotalPrice = 98 };

            _productRepositoryMock
                .Setup(r => r.CheckInternalSystemProduct(order.ProductId))
                .ReturnsAsync(true);

            _productRepositoryMock
                .Setup(r => r.Chekout(order))
                .ReturnsAsync(new ProductEntity { availableQuantity = 5 });

            // Act
            var result = await _productServiceIMPL.GetCheckout(order);

            // Assert
            Assert.True(result);
            }

        [Fact]
        public async Task GetCheckout_InternalProduct_WithInsufficientQuantity_ReturnsFalse()
            {
            // Arrange
            var order = new CheckoutDTO { ProductId = 1, quantity = 10, itemTotalPrice = 98 };

            _productRepositoryMock
                .Setup(r => r.CheckInternalSystemProduct(order.ProductId))
                .ReturnsAsync(true);

            _productRepositoryMock
                .Setup(r => r.Chekout(order))
                .ReturnsAsync(new ProductEntity { availableQuantity = 5 });

            // Act
            var result = await _productServiceIMPL.GetCheckout(order);

            // Assert
            Assert.False(result);
            }

        [Fact]
        public async Task GetCheckout_ExternalProduct_WithSuccessfulAdapterCall_ReturnsTrue()
            {
            // Arrange
            var order = new CheckoutDTO { ProductId = 2, quantity = 1 };

            _productRepositoryMock
                .Setup(r => r.CheckInternalSystemProduct(order.ProductId))
                .ReturnsAsync(false);

            var response = new HttpResponseMessage(HttpStatusCode.OK);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = await _productServiceIMPL.GetCheckout(order);

            // Assert
            Assert.True(result);
            }

        [Fact]
        public async Task GetCheckout_ExternalProduct_WithFailedAdapterCall_ReturnsFalse()
            {
            // Arrange
            var order = new CheckoutDTO { ProductId = 2, quantity = 1 };

            _productRepositoryMock
                .Setup(r => r.CheckInternalSystemProduct(order.ProductId))
                .ReturnsAsync(false);

            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = await _productServiceIMPL.GetCheckout(order);

            // Assert
            Assert.False(result);
            }
        [Fact]
        public async Task GetCheckout_WhenExceptionThrown_LogsErrorAndThrows()
            {
            // Arrange
            var order = new CheckoutDTO { ProductId = 1, quantity = 1 };

            _productRepositoryMock
                .Setup(r => r.CheckInternalSystemProduct(order.ProductId))
                .ThrowsAsync(new Exception("Repo error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _productServiceIMPL.GetCheckout(order));
            Assert.Equal("error occur", ex.Message);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            }

        [Fact]
        public async Task UpdateProduct_WhenProductExistsAndUpdateSucceeds_ReturnsTrue()
            {
            // Arrange
            var productId = 1L;
            var productDto = new ProductDTO
                {
                Id = productId,
                Name = "Updated Product",
                Description = "New Description",
                Price = 100,
                Currency = "USD",
                originId = 1,
                provider = "TestProvider",
                availableQuantity = 10,
                owner = "owner123",
                Attributes = new List<ProductAttributesDTO>
            {
                new ProductAttributesDTO { provider = "prov1", Key = "Color", Value = "Red" }
            },
                Contents = new List<ProductContentDTO>
            {
                new ProductContentDTO { provider = "prov1", Type = "image", Url = "url", Description = "desc" }
            }
                };

            var existingProduct = new ProductEntity
                {
                Id = productId,
                Attributes = new List<ProductAttributesEntity>(),
                Contents = new List<ProductContentEntity>()
                };

            _productRepositoryMock.Setup(r => r.GetProductById(productId))
                .ReturnsAsync(existingProduct);

            _productRepositoryMock.Setup(r => r.RemoveAllProductAttributesByProvider(It.IsAny<ProductEntity>()))
                .Returns(Task.CompletedTask);

            _productRepositoryMock.Setup(r => r.RemoveAllProductContentsWhereProviderNotEmpty(It.IsAny<ProductEntity>()))
                .Returns(Task.CompletedTask);

            _productRepositoryMock.Setup(r => r.UpdateProductAsync(It.IsAny<ProductEntity>()))
                .ReturnsAsync(1);

            // Act
            var result = await _productServiceIMPL.UpdateProduct(productDto);

            // Assert
            Assert.True(result);
            }

        [Fact]
        public async Task UpdateProduct_WhenProductNotFound_ReturnsFalse()
            {
            // Arrange
            _productRepositoryMock.Setup(r => r.GetProductById(It.IsAny<long>()))
                .ReturnsAsync((ProductEntity)null);

            var productDto = new ProductDTO { Id = 99 };

            // Act
            var result = await _productServiceIMPL.UpdateProduct(productDto);

            // Assert
            Assert.False(result);
            }

        [Fact]
        public async Task UpdateProduct_WhenExceptionThrown_LogsErrorAndThrows()
            {
            // Arrange
            _productRepositoryMock.Setup(r => r.GetProductById(It.IsAny<long>()))
                .ThrowsAsync(new Exception("DB error"));

            var productDto = new ProductDTO { Id = 1 };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _productServiceIMPL.UpdateProduct(productDto));
            Assert.Equal("error occur", ex.Message);

            _loggerMock.Verify(
                  x => x.Log(
                  LogLevel.Error,
                  It.IsAny<EventId>(),
                  It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred")),
                  It.IsAny<Exception>(),
                  It.IsAny<Func<It.IsAnyType, Exception, string>>()
                            ), Times.Once);

            }

        }


        }

