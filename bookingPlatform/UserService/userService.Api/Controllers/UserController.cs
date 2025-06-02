using Microsoft.AspNetCore.Mvc;
using userService.Api.DTO;
using userService.Api.service.interfaces;

namespace userService.Api.Controllers
{
    [Route("api/v1/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>UserDTO</returns>
        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetUser([FromQuery] long id)
        {
            try
            {
                var user = await _userService.GetUsersAsync(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">UserDTO object</param>
        /// <returns>Success status</returns>
        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] UserDTO user)
        {
            try
            {
                var success = await _userService.CreateUserAsync(user);
                if (success)
                    return Ok(new { message = "User created successfully." });
                else
                    return BadRequest(new { message = "User creation failed." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Logs in a user with username and password.
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Login status</returns>
        [HttpPost("login")]
        public async Task<ActionResult> LoginUser([FromQuery] string userName, [FromQuery] string password)
        {
            try
            {
                UserDTO result = await _userService.LoginUserAsync(userName, password);
                return Ok(result.userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
