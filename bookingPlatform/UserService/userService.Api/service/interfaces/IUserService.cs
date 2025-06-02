using userService.Api.DTO;

namespace userService.Api.service.interfaces
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(UserDTO user);
        Task<UserDTO> LoginUserAsync(string userName, string password);
        Task<UserDTO> GetUsersAsync(long userId);
    }
}
