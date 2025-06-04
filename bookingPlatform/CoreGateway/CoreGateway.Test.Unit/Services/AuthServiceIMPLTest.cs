using CoreGateway.API.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using Xunit;
namespace CoreGatewayService.Test.Unit.Services
    {
    public class AuthServiceIMPLTest
        {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IOptions<ServiceUrls>> _optionsMock;
        private readonly AuthServiceIMPL _authService;

        public AuthServiceIMPLTest()
            {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(config => config["Jwt:Key"]).Returns("ThisIsASecretKeyWithAtLeastThirtyTwoChars!");
            _configurationMock.Setup(config => config["Jwt:Issuer"]).Returns("myIssuer");
            _configurationMock.Setup(config => config["Jwt:Audience"]).Returns("myAudience");


            _optionsMock = new Mock<IOptions<ServiceUrls>>();
            _optionsMock.Setup(o => o.Value).Returns(new ServiceUrls
                {
                UserService = "https://fake-auth-service.com"
                });

            _authService = new AuthServiceIMPL(_httpClient, _configurationMock.Object, _optionsMock.Object);
            }
        [Fact]
        public void GenerateJwtToken_ValidUsername_ReturnsToken()
            {
            // Arrange
            string username = "testuser";

            // Act
            var token = _authService.GenerateJwtToken(username);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));

            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(token));

            var jwtToken = handler.ReadJwtToken(token);

            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == username);
            Assert.Equal("myIssuer", jwtToken.Issuer);
            Assert.Contains("myAudience", jwtToken.Audiences);

          //  Assert.True(jwtToken.ValidTo > DateTime.Now);
            }
        [Fact]
        public void GenerateJwtToken_WhenConfigurationIsInvalid_ThrowsException()
            {
            // Arrange: Setup configuration to return null for the secret key, which causes failure
            _configurationMock.Setup(config => config["Jwt:Key"]).Returns<string>(null);

            // Re-create the service with this invalid configuration mock
            var authServiceWithInvalidConfig = new AuthServiceIMPL(new System.Net.Http.HttpClient(), _configurationMock.Object, _optionsMock.Object);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => authServiceWithInvalidConfig.GenerateJwtToken("testuser"));
            Assert.Equal("Error generating JWT token", exception.Message);
            Assert.NotNull(exception.InnerException);
            }
        [Fact]
        public async Task ValidateUserCredentials_Success_ReturnsTokenDTO()
            {
            // Arrange
            var userName = "testuser";
            var password = "testpass";
            var userIdString = "12345";

            var url = $"https://fake-auth-service.com/api/v1/user/login?userName={Uri.EscapeDataString(userName)}&password={Uri.EscapeDataString(password)}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post && req.RequestUri.ToString() == url),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                    {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(userIdString),
                    })
                .Verifiable();

            // Act
            var result = await _authService.ValidateUserCredentials(userName, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(12345, result.userId);
            Assert.False(string.IsNullOrEmpty(result.acessToken));
            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(result.acessToken));

            var jwtToken = handler.ReadJwtToken(result.acessToken);

            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userName);

            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post && req.RequestUri.ToString() == url),
                ItExpr.IsAny<CancellationToken>()
            );
            }

        [Fact]
        public async Task ValidateUserCredentials_Failure_ReturnsEmptyTokenDTO()
            {
            // Arrange
            var userName = "wronguser";
            var password = "wrongpass";

            var url = $"https://fake-auth-service.com/api/v1/user/login?userName={Uri.EscapeDataString(userName)}&password={Uri.EscapeDataString(password)}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post && req.RequestUri.ToString() == url),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                    {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("Unauthorized"),
                    })
                .Verifiable();

            // Act
            var result = await _authService.ValidateUserCredentials(userName, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.userId);
            Assert.True(string.IsNullOrEmpty(result.acessToken));

            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post && req.RequestUri.ToString() == url),
                ItExpr.IsAny<CancellationToken>()
            );
            }

        [Fact]
        public async Task ValidateUserCredentials_ExceptionThrown_ThrowsUnauthorizedException()
            {
            // Arrange
            var userName = "testuser";
            var password = "testpass";

            var url = $"https://fake-auth-service.com/api/v1/user/login?userName={Uri.EscapeDataString(userName)}&password={Uri.EscapeDataString(password)}";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"))
                .Verifiable();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _authService.ValidateUserCredentials(userName, password));
            Assert.Equal("unauthorized", exception.Message);

            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
            }

        }
    }
