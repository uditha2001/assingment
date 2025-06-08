using Microsoft.Extensions.Logging;
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
    public class ProductAttributeServiceIMPLTest
        {
          private readonly ProductAttributeServiceIMPL _service;
          private readonly Mock<IProductAttriuteRepository> _mockRepo;
          private readonly Mock<ILogger<ProductAttributeServiceIMPL>> _loggerMock;
          
        public ProductAttributeServiceIMPLTest()
            {
                _mockRepo = new Mock<IProductAttriuteRepository>();
                _loggerMock = new Mock<ILogger<ProductAttributeServiceIMPL>>();
                _service = new ProductAttributeServiceIMPL(_mockRepo.Object, _loggerMock.Object);
            }

        [Fact]
        public async Task CreateAttribute_ValidInput_ReturnsTrue()
            {
            // Arrange
            var productId = 123;
            var dtos = new List<ProductAttributesDTO>
    {
        new ProductAttributesDTO { /* initialize properties if needed */ },
        new ProductAttributesDTO { /* initialize properties if needed */ }
    };

            // Setup repo mock to return true when CreateAttributes is called with any list
            _mockRepo.Setup(r => r.CreateAttributes(It.IsAny<List<ProductAttributesEntity>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.CreateAttribute(dtos, productId);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(r => r.CreateAttributes(It.Is<List<ProductAttributesEntity>>(list => list.Count == dtos.Count)), Times.Once);
            }

        [Fact]
        public async Task CreateAttribute_NullInput_ReturnsFalse()
            {
            // Arrange
            List<ProductAttributesDTO>? dtos = null;
            var productId = 123;

            // Act
            var result = await _service.CreateAttribute(dtos, productId);

            // Assert
            Assert.False(result);
            _mockRepo.Verify(r => r.CreateAttributes(It.IsAny<List<ProductAttributesEntity>>()), Times.Never);
            }

        [Fact]
        public async Task CreateAttribute_WhenExceptionThrown_ReturnsFalseAndLogsError()
            {
            // Arrange
            var productId = 123;
            var dtos = new List<ProductAttributesDTO>
    {
        new ProductAttributesDTO { /* init */ }
    };

            _mockRepo.Setup(r => r.CreateAttributes(It.IsAny<List<ProductAttributesEntity>>()))
                .ThrowsAsync(new Exception("DB failure"));

            // Act
            var result = await _service.CreateAttribute(dtos, productId);

            // Assert
            Assert.False(result);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while processing the request.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            }

        [Fact]
        public async Task DeleteAttribute_ValidId_ReturnsTrue()
            {
            // Arrange
            long attributeId = 10;
            _mockRepo.Setup(r => r.DeleteAttribute(attributeId)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAttribute(attributeId);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(r => r.DeleteAttribute(attributeId), Times.Once);
            }

        [Fact]
        public async Task DeleteAttribute_WhenExceptionThrown_ReturnsFalseAndLogsError()
            {
            // Arrange
            long attributeId = 10;
            _mockRepo.Setup(r => r.DeleteAttribute(attributeId)).ThrowsAsync(new Exception("DB failure"));

            // Act
            var result = await _service.DeleteAttribute(attributeId);

            // Assert
            Assert.False(result);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while processing the request.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            }
        [Fact]
        public async Task GetAllAttributes_ValidProductId_ReturnsDTOList()
            {
            // Arrange
            long productId = 1;
            var fakeEntities = new List<ProductAttributesEntity>
    {
        new ProductAttributesEntity { AttributeId = 101, Key = "Color", Value = "Red", ProductId = productId },
        new ProductAttributesEntity { AttributeId = 102, Key = "Size", Value = "Large", ProductId = productId }
    };

            _mockRepo.Setup(r => r.GetAllAttributes(productId)).ReturnsAsync(fakeEntities);

            // Act
            var result = await _service.GetAllAttributes(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fakeEntities.Count, result.Count);
            Assert.All(result, item => Assert.Contains(fakeEntities, e => e.AttributeId == item.attributeId && e.Key == item.Key));
            }

        [Fact]
        public async Task GetAllAttributes_WhenExceptionThrown_LogsErrorAndThrows()
            {
            // Arrange
            long productId = 1;
            var exceptionMessage = "DB failure";
            _mockRepo.Setup(r => r.GetAllAttributes(productId)).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetAllAttributes(productId));
            Assert.Contains(exceptionMessage, ex.Message);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while processing the request.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            }

        [Fact]
        public async Task UpdateAttribute_ValidInput_ReturnsTrue()
            {
            // Arrange
            long productId = 1;
            var dtos = new List<ProductAttributesDTO>
    {
        new ProductAttributesDTO { attributeId = 101, Key = "Color", Value = "Blue" },
        new ProductAttributesDTO { attributeId = 102, Key = "Size", Value = "Medium" }
    };

            // Setup repository to return true
            _mockRepo.Setup(r => r.UpdateAttributesAsync(It.IsAny<List<ProductAttributesEntity>>()))
                     .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateAttribute(dtos, productId);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(r => r.UpdateAttributesAsync(It.Is<List<ProductAttributesEntity>>(list => list.Count == dtos.Count)), Times.Once);
            }

        [Fact]
        public async Task UpdateAttribute_WhenExceptionThrown_ReturnsFalseAndLogsError()
            {
            // Arrange
            long productId = 1;
            var dtos = new List<ProductAttributesDTO>
    {
            new ProductAttributesDTO { attributeId = 101, Key = "Color", Value = "Blue" }
    };
            var exception = new Exception("DB failure");

            _mockRepo.Setup(r => r.UpdateAttributesAsync(It.IsAny<List<ProductAttributesEntity>>()))
                     .ThrowsAsync(exception);

            // Act
            var result = await _service.UpdateAttribute(dtos, productId);

            // Assert
            Assert.False(result);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while processing the request.")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            }





        }
    }
