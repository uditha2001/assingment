using userService.Api.DTO;

namespace userService.Api.service.interfaces
{
    public interface IUserService
    {
        Task<bool> createUserAsync(UserDTO user);
        Task<UserDTO> loginUserAsync(string userName, string password);
        Task<UserDTO> getUsersAsync(long userId);
    }
}
