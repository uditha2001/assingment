using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AdapterFactory.Adapters;
using AdapterFactory.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using OrderService.API.DTO;
using ProductService.API.DTO;
using Xunit;

namespace AdapterService.Test.Unit.Services
{
    public class AdapterFactoryServiceIMPLTest
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<AdapterFactoryServiceIMPL>> _loggerMock;
        private readonly Mock<IOptions<ServiceUrls>> _optionsMock;
        private readonly ServiceUrls _serviceUrls;
        private readonly Mock<IAdapter> _adapterMock;
        private readonly AdapterFactoryServiceIMPL _service;

        public AdapterFactoryServiceIMPLTest()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _loggerMock = new Mock<ILogger<AdapterFactoryServiceIMPL>>();
            _serviceUrls = new ServiceUrls { ProductService = "http://test-product-service" };
            _optionsMock = new Mock<IOptions<ServiceUrls>>();
            _optionsMock.Setup(o => o.Value).Returns(_serviceUrls);

            _adapterMock = new Mock<IAdapter>();
            _adapterMock.Setup(a => a.SourceName).Returns("TestProvider");
            _adapterMock.Setup(a => a.GetProductContentsFromExternalServiceAsync())
                .ReturnsAsync(new List<ProductDTO> { new ProductDTO { Id = 1, provider = "TestProvider" } });
            _adapterMock.Setup(a => a.Checkout()).Returns(true);

            _service = new AdapterFactoryServiceIMPL(
                _httpClient,
                _loggerMock.Object,
                _optionsMock.Object,
                new List<IAdapter> { _adapterMock.Object }
            );
        }

        [Fact]
        public void GetAdapterById_ReturnsAdapter_WhenExists()
        {
            var adapter = _service.GetAdapterById("TestProvider");
            Assert.NotNull(adapter);
            Assert.Equal("TestProvider", adapter.SourceName);
        }

        [Fact]
        public void GetAdapterById_Throws_WhenNotFound()
        {
            Assert.Throws<KeyNotFoundException>(() => _service.GetAdapterById("Unknown"));
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsProducts()
        {
            var result = await _service.GetAllProductsAsync();
            Assert.Single(result);
            Assert.Equal("TestProvider", result[0].provider);
        }

        [Fact]
        public async Task PlaceOrder_ReturnsTrue_OnSuccess()
        {
            var product = new ProductDTO { Id = 1, provider = "TestProvider" };
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json")
            };
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var order = new CheckoutDTO { ProductId = 1 };
            var result = await _service.PlaceOrder(order);
            Assert.True(result);
        }

        [Fact]
        public async Task PlaceOrder_Throws_OnHttpError()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var order = new CheckoutDTO { ProductId = 1 };
            await Assert.ThrowsAsync<HttpRequestException>(() => _service.PlaceOrder(order));
        }

        [Fact]
        public async Task CheckoutOrder_ReturnsTrue_OnSuccess()
        {
            var product = new ProductDTO { Id = 1, provider = "TestProvider" };
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json")
            };
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var order = new CheckoutDTO { ProductId = 1 };
            var result = await _service.CheckoutOrder(order);
            Assert.True(result);
        }

        [Fact]
        public async Task CheckoutOrder_Throws_OnMissingProvider()
        {
            var product = new ProductDTO { Id = 1, provider = string.Empty };
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json")
            };
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var order = new CheckoutDTO { ProductId = 1 };
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CheckoutOrder(order));
        }
    }
}
