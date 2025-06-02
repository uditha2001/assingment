using CoreGateway.API.dto;
using CoreGateway.API.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreGateway.API.Controllers
{
    [ApiController]
    [Route("api/v1/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        // [HttpPost("login")]
        //public IActionResult Login([FromBody] UserLoginInfo user)
        //{
        //    if (_authService.ValidateUserCredentials(user., user.Password))
        //    {
        //        var token = _authService.GenerateJwtToken(user.Username);
        //        return Ok(new { token });
        //    }
        //    return Unauthorized();
        //}
        [HttpPost]
        public async Task<IActionResult> LoginUser([FromQuery] string userName, [FromQuery] string password)
        {
            try
            {
                TokenDTO token = await _authService.ValidateUserCredentials(userName, password);
                if (!String.IsNullOrEmpty(token.acessToken))
                {
                    return Ok(token);

                }
                else
                {
                    return BadRequest("invilid credentials");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("invilid credentials");
            }

        }
    }
}
