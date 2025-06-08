using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ProductService.API.DTO;
using ProductService.API.Models.Entities;
using ProductService.API.Repository.RepositoryInterfaces;
using ProductService.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectService.Test.Unit.Services
    {
    public class ProductContentServiceIMPLTest
        {
        private readonly Mock<IProductContentRepository> _repositoryMock;
        private readonly ProductContentServiceIMPL _service;
        private readonly Mock<ILogger<ProductContentServiceIMPL>> _loggerMock;
        private readonly Mock<IWebHostEnvironment> _envMock;

        public ProductContentServiceIMPLTest()
            {
            _repositoryMock = new Mock<IProductContentRepository>();
            _loggerMock = new Mock<ILogger<ProductContentServiceIMPL>>();
            _envMock = new Mock<IWebHostEnvironment>();
            _envMock.Setup(e => e.EnvironmentName).Returns("Development");
            _envMock.Setup(e => e.WebRootPath).Returns("wwwroot");
            _envMock.Setup(e => e.ContentRootPath).Returns("C:\\Projects\\MyApp");
            _service = new ProductContentServiceIMPL(_repositoryMock.Object, _loggerMock.Object, _envMock.Object);

            }
        [Fact]
        public async Task CreateContent_WhenCalled_ReturnsTrue()
            {
            // Arrange
            var productContentDTO = new ProductContentDTO();
            long productId = 1;
            _repositoryMock
                .Setup(r => r.CreateContentAsync(It.IsAny<ProductContentEntity>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.CreateContent(productContentDTO, productId);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.CreateContentAsync(It.IsAny<ProductContentEntity>()), Times.Once);
            _loggerMock.VerifyNoOtherCalls();
            }

        [Fact]
        public async Task CreateContent_WhenExceptionThrown_LogsErrorAndReturnsFalse()
            {
            // Arrange
            var productContentDTO = new ProductContentDTO();
            long productId = 1;
            _repositoryMock
                .Setup(r => r.CreateContentAsync(It.IsAny<ProductContentEntity>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.CreateContent(productContentDTO, productId);

            // Assert
            Assert.False(result);
            _loggerMock.Verify(
               x => x.Log(
               LogLevel.Error,
               It.IsAny<EventId>(),
               It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error creating product content")),
               It.IsAny<Exception>(),
               It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.Once);

            }
        [Fact]
        public async Task GetAllContent_WhenCalled_ReturnsMappedContentList()
            {
            // Arrange
            long productId = 1;
            var fakeEntities = new List<ProductContentEntity>
              {
                 new ProductContentEntity { ContentId = 1, ProductId = productId, Url = "http://example.com",provider="test" },
                 new ProductContentEntity { ContentId = 2, ProductId = productId, Url = "http://example2.com",provider = "test" }
             };

            _repositoryMock
                .Setup(r => r.GetAllContentAsync(productId))
                .ReturnsAsync(fakeEntities);

            // Act
            var result = await _service.GetAllContent(productId);

            // Assert
            for (int i = 0; i < fakeEntities.Count; i++)
                {
                Assert.Equal(fakeEntities[i].ContentId, result[i].contentId);
                Assert.Equal(fakeEntities[i].Url, result[i].Url);
                Assert.Equal(fakeEntities[i].Description, result[i].Description);
                }

            }
        [Fact]
        public async Task GetAllContent_WhenExceptionThrown_LogsErrorAndReturnsEmptyList()
            {
            // Arrange
            long productId = 1;

            _repositoryMock
                .Setup(r => r.GetAllContentAsync(productId))
                .ThrowsAsync(new Exception("DB failure"));

            // Act
            var result = await _service.GetAllContent(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _loggerMock.Verify(
                 x => x.Log<It.IsAnyType>(
                 LogLevel.Error,
                 It.IsAny<EventId>(),
                 It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error retrieving all product content")),
                 It.IsAny<Exception>(),
                 It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                 Times.Once
 );


            }

        [Fact]
        public async Task SaveProductImagesAsync_ValidImages_SavesFilesAndReturnsTrue()
            {
            // Arrange
            var productId = 123;
            var mockFile = new Mock<IFormFile>();
            var content = "Fake file content";
            var fileName = "test.png";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(ms.Length);
            mockFile.Setup(f => f.ContentType).Returns("image/png");
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream stream, CancellationToken token) => ms.CopyToAsync(stream));

            var imageList = new List<IFormFile> { mockFile.Object };

            var rootPath = Path.Combine(Path.GetTempPath(), "testwwwroot");
            if (Directory.Exists(rootPath)) Directory.Delete(rootPath, true);

            _envMock.Setup(e => e.WebRootPath).Returns(rootPath);

            var serviceMock = new Mock<ProductContentServiceIMPL>(_repositoryMock.Object, _loggerMock.Object, _envMock.Object)
                {
                CallBase = true
                };

            serviceMock.Setup(s => s.CreateContent(It.IsAny<ProductContentDTO>(), productId)).ReturnsAsync(true);

            // Act
            var result = await serviceMock.Object.SaveProductImagesAsync(productId, imageList);

            // Assert
            Assert.True(result);

            var savedPath = Path.Combine(rootPath, "uploads", "products", productId.ToString());
            Assert.True(Directory.Exists(savedPath));
            Assert.True(Directory.GetFiles(savedPath).Length == 1);

            // Cleanup
            Directory.Delete(rootPath, true);
            }

        [Fact]
        public async Task SaveProductImagesAsync_WhenExceptionThrown_ReturnsFalseAndLogsError()
            {
            // Arrange
            var productId = 1;
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("test.jpg");
            formFileMock.Setup(f => f.Length).Returns(10);
            formFileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                        .ThrowsAsync(new IOException("Simulated failure"));

            var images = new List<IFormFile> { formFileMock.Object };

            var wwwrootPath = Path.Combine(Path.GetTempPath(), "test_wwwroot_exception");
            _envMock.Setup(e => e.WebRootPath).Returns(wwwrootPath);

            var service = new ProductContentServiceIMPL(_repositoryMock.Object, _loggerMock.Object, _envMock.Object);

            // Act
            var result = await service.SaveProductImagesAsync(productId, images);

            // Assert
            Assert.False(result);

            _loggerMock.Verify(
                 x => x.Log(
                 LogLevel.Error,
                 It.IsAny<EventId>(),
                 It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error saving content")),
                 It.IsAny<Exception>(),
                 It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                 Times.Once);


            // Cleanup
            if (Directory.Exists(wwwrootPath))
                Directory.Delete(wwwrootPath, true);
            }





        }
    }
