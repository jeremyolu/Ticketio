using Ticketio.Auth.Api.Models.Data;
using Ticketio.Auth.Api.Models.Requests;
using Ticketio.Auth.Api.Models.Responses;

namespace Ticketio.Auth.Api.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponse<string>> Register(RegisterRequest request);
    Task<AuthResponse<AuthToken>> Login(AuthRequest request);
    Task<AuthResponse<AuthToken>> Refresh(TokenRequest request);
}
