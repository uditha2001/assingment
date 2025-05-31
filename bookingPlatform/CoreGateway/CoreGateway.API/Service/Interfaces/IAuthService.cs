using CoreGateway.API.dto;

namespace CoreGateway.API.Service.Interfaces
{
    public interface IAuthService
    {
        string GenerateJwtToken(string username);
        Task<TokenDTO> ValidateUserCredentials(string userName, string password);    }
}
