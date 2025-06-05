using Moq;
using System;
using System.Threading.Tasks;
using userService.Api.DTO;
using userService.Api.entity;
using userService.Api.repository.interfaces;
using userService.Api.service;
using Xunit;

namespace UserService.Test.Unit.Services
{
    public class UserServiceIMPLTest
    {
        private readonly UserServiceIMPL _userService;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        public UserServiceIMPLTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserServiceIMPL(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsTrue_WhenUserIsCreated()
        {
            var userDto = new UserDTO
            {
                userId = 1,
                userName = "testuser",
                password = "password",
                Email = "test@mail.com",
                FirstName = "Test",
                LastName = "User"
            };

            _userRepositoryMock
                .Setup(r => r.CreateUserAsync(It.IsAny<UserEntity>()))
                .ReturnsAsync(true);

            var result = await _userService.CreateUserAsync(userDto);

            Assert.True(result);
        }

        [Fact]
        public async Task CreateUserAsync_ThrowsArgumentNullException_WhenUserIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.CreateUserAsync(null));
        }

        [Fact]
        public async Task CreateUserAsync_ThrowsArgumentException_WhenRequiredFieldsMissing()
        {
            var userDto = new UserDTO { userName = "", password = "", Email = "" };
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateUserAsync(userDto));
        }

        [Fact]
        public async Task CreateUserAsync_ThrowsInvalidOperationException_WhenRepositoryReturnsFalse()
        {
            var userDto = new UserDTO
            {
                userId = 1,
                userName = "testuser",
                password = "password",
                Email = "test@mail.com",
                FirstName = "Test",
                LastName = "User"
            };

            _userRepositoryMock
                .Setup(r => r.CreateUserAsync(It.IsAny<UserEntity>()))
                .ReturnsAsync(false);

            var ex = await Assert.ThrowsAsync<Exception>(() => _userService.CreateUserAsync(userDto));
            Assert.Contains("User creation failed", ex.Message);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsUserDTO_WhenUserExists()
        {
            var userEntity = new UserEntity
            {
                userId = 1,
                userName = "testuser",
                password = "hashed",
                firstName = "Test",
                lastName = "User",
                Email = "test@mail.com"
            };

            _userRepositoryMock
                .Setup(r => r.GetUsersAsync(1))
                .ReturnsAsync(userEntity);

            var result = await _userService.GetUsersAsync(1);

            Assert.NotNull(result);
            Assert.Equal(userEntity.userId, result.userId);
        }

        [Fact]
        public async Task GetUsersAsync_ThrowsException_WhenUserNotFound()
        {
            _userRepositoryMock
                .Setup(r => r.GetUsersAsync(It.IsAny<long>()))
                .ReturnsAsync((UserEntity)null);

            var ex = await Assert.ThrowsAsync<Exception>(() => _userService.GetUsersAsync(1));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsUserDTO_WhenCredentialsAreValid()
        {
            var password = "password";
            var hashed = BCrypt.Net.BCrypt.HashPassword(password);

            var userEntity = new UserEntity
            {
                userId = 1,
                userName = "testuser",
                password = hashed,
                firstName = "Test",
                lastName = "User",
                Email = "test@mail.com"
            };

            _userRepositoryMock
                .Setup(r => r.LoginUserAsync("testuser", password))
                .ReturnsAsync(userEntity);

            var result = await _userService.LoginUserAsync("testuser", password);

            Assert.NotNull(result);
            Assert.Equal(userEntity.userName, result.userName);
        }

        [Fact]
        public async Task LoginUserAsync_ThrowsException_WhenUserNotFound()
        {
            _userRepositoryMock
                .Setup(r => r.LoginUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((UserEntity)null);

            var ex = await Assert.ThrowsAsync<Exception>(() => _userService.LoginUserAsync("testuser", "password"));
            Assert.Contains("user not found", ex.Message);
        }

        [Fact]
        public async Task LoginUserAsync_ThrowsException_WhenPasswordInvalid()
        {
            var userEntity = new UserEntity
            {
                userId = 1,
                userName = "testuser",
                password = BCrypt.Net.BCrypt.HashPassword("otherpassword"),
                firstName = "Test",
                lastName = "User",
                Email = "test@mail.com"
            };

            _userRepositoryMock
                .Setup(r => r.LoginUserAsync("testuser", "password"))
                .ReturnsAsync(userEntity);

            var ex = await Assert.ThrowsAsync<Exception>(() => _userService.LoginUserAsync("testuser", "password"));
            Assert.Contains("invailid password", ex.Message);
        }

        [Fact]
        public void ToDTO_MapsEntityToDTO()
        {
            var entity = new UserEntity
            {
                userId = 1,
                userName = "testuser",
                password = "hashed",
                firstName = "Test",
                lastName = "User",
                Email = "test@mail.com"
            };

            var dto = _userService.ToDTO(entity);

            Assert.Equal(entity.userId, dto.userId);
            Assert.Equal(entity.userName, dto.userName);
            Assert.Equal(entity.password, dto.password);
            Assert.Equal(entity.firstName, dto.FirstName);
            Assert.Equal(entity.lastName, dto.LastName);
            Assert.Equal(entity.Email, dto.Email);
        }

        [Fact]
        public void ToEntity_MapsDTOToEntity()
        {
            var dto = new UserDTO
            {
                userId = 1,
                userName = "testuser",
                password = "hashed",
                FirstName = "Test",
                LastName = "User",
                Email = "test@mail.com"
            };

            var entity = _userService.ToEntity(dto);

            Assert.Equal(dto.userId, entity.userId);
            Assert.Equal(dto.userName, entity.userName);
            Assert.Equal(dto.password, entity.password);
            Assert.Equal(dto.FirstName, entity.firstName);
            Assert.Equal(dto.LastName, entity.lastName);
            Assert.Equal(dto.Email, entity.Email);
        }
    }
}
