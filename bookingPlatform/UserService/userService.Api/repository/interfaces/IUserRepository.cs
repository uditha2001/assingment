using userService.Api.DTO;
using userService.Api.entity;

namespace userService.Api.repository.interfaces
{
    public interface IUserRepository
    {
        Task<bool> CreateUserAsync(UserEntity user);
        Task<UserEntity> LoginUserAsync(string userName, string password);
        Task<UserEntity> GetUsersAsync(long userId);
    }
}
