using Ticketio.Auth.Api.Models.Data;
using Ticketio.Auth.Api.Models.Requests;
using Ticketio.Core.Models.Responses;

namespace Ticketio.Auth.Api.Interfaces.Services;

public interface IAuthService
{
    Task<ResultResponse<string>> Register(RegisterRequest request);
    Task<ResultResponse<AuthToken>> Login(AuthRequest request);
    Task<ResultResponse<AuthToken>> Refresh(TokenRequest request);
    Task<ResultResponse<bool>> ForgotPassword(ForgotPasswordRequest request);
    Task<ResultResponse<bool>> ValidateResetToken(string token);
    Task<ResultResponse<bool>> ResetPassword(ResetPasswordRequest request);
}
