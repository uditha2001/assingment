using userService.Api.DTO;
using userService.Api.entity;

namespace userService.Api.repository.interfaces
{
    public interface IUserRepository
    {
        Task<bool> createUserAsync(UserEntity user);
        Task<UserEntity> loginUserAsync(string userName, string password);
        Task<UserEntity> getUsersAsync(long userId);
    }
}
