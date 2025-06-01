using CoreGateway.API.dto;
using CoreGateway.API.Service.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace CoreGateway.API.Service
{
    public class AuthServiceIMPL : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ServiceUrls _urls;


        public AuthServiceIMPL(HttpClient httpClient, IConfiguration configuration, IOptions<ServiceUrls> options)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _urls = options.Value;
        }

        public string GenerateJwtToken(string username)
        {
            var secret = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var claims = new[]
                  {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<TokenDTO> ValidateUserCredentials(string userName, string password)
        {
            try
            {
                var url = $"{_urls.UserService}/api/v1/user/login?userName={Uri.EscapeDataString(userName)}&password={Uri.EscapeDataString(password)}";
                var response = await _httpClient.PostAsync(url, null);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    long result = long.Parse(content);                    
                    String acessToken= GenerateJwtToken(userName);
                    TokenDTO tokenDTO = new TokenDTO();
                    tokenDTO.acessToken = acessToken;
                    tokenDTO.userId = result;
                    return tokenDTO;
                
                }
                else
                {
                    return new TokenDTO();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("unauthorized");
            }
            
        }

    }
}
