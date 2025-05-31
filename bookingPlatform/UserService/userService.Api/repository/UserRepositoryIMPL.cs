using Microsoft.EntityFrameworkCore;
using userService.Api.Data;
using userService.Api.DTO;
using userService.Api.entity;
using userService.Api.repository.interfaces;

namespace userService.Api.repository
{
    public class UserRepositoryIMPL : IUserRepository
    {
        private readonly UserDbContext _userDbContext;

        public UserRepositoryIMPL(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        public async Task<bool> createUserAsync(UserEntity user)
        {
            try
            {
                _userDbContext.Users.Add(user);
                await _userDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<UserEntity> getUsersAsync(long userId)
        {
            var user = await _userDbContext.Users.FindAsync(userId);
            return user;
        }

        public async Task<UserEntity> loginUserAsync(string userName, string password)
        {
          UserEntity userDetails=await _userDbContext.Users.FirstOrDefaultAsync(U=>U.userName== userName);
            return userDetails;
        }
    }
}
