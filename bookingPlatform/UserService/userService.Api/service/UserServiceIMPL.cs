using System;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using userService.Api.DTO;
using userService.Api.entity;
using userService.Api.repository.interfaces;
using userService.Api.service.interfaces;

namespace userService.Api.service
{
    public class UserServiceIMPL : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserServiceIMPL(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> CreateUserAsync(UserDTO user)
        {
            user.password=BCrypt.Net.BCrypt.HashPassword(user.password);
            var entity = ToEntity(user);
            var result = await _userRepository.CreateUserAsync(entity);

            if (!result)
                throw new Exception("User creation failed.");

            return result;
        }

        public async Task<UserDTO> GetUsersAsync(long userId)
        {
            var user = await _userRepository.GetUsersAsync(userId);

            if (user == null)
                throw new Exception($"User with ID {userId} not found.");

            return ToDTO(user);
        }

        public async Task<UserDTO> LoginUserAsync(string userName, string password)
        {
            UserEntity user = await _userRepository.LoginUserAsync(userName, password);
            if (user == null)
                throw new Exception("user not found");

            bool isPasswordVailid = BCrypt.Net.BCrypt.Verify(password,user.password);
            if (!isPasswordVailid)
            {
                throw new Exception("invailid password");
            }
            else
            {
                UserDTO userDto=ToDTO(user);
                return userDto ;
            }
            
        }

        public UserDTO ToDTO(UserEntity entity)
        {
            return new UserDTO
            {
                userId = entity.userId,
                userName = entity.userName,
                password = entity.password,
                FirstName = entity.firstName,
                LastName = entity.lastName,
                Email=entity.Email,
            };
        }

        public UserEntity ToEntity(UserDTO dto)
        {
            return new UserEntity
            {
                userId = dto.userId,
                userName = dto.userName,
                password = dto.password,
                firstName = dto.FirstName,
                lastName = dto.LastName,
                Email = dto.Email
            };
        }
    }
}
