using System.ComponentModel.DataAnnotations;

namespace userService.Api.DTO
{
    public class UserDTO
    {
        public long userId { get; set; }
        public string userName {  get; set; }
        public string password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
