using Ticketio.Auth.Api.Enums;
using Ticketio.Auth.Api.Models.Data;
using Ticketio.Auth.Api.Models.Requests;

namespace Ticketio.Auth.Api.Interfaces.Repositories;

public interface IAuthRepository
{
    Task<User?> GetUserById(Guid id);
    Task<User?> GetUserByEmail(string email);
    Task<bool> RegisterUser(RegisterRequest request);
    Task<bool> UpdateUserPassword(Guid id, string password);
    Task<bool> SaveToken(RefreshToken token);
    Task<RefreshToken?> GetToken(string refreshToken);
    Task<ResetToken?> GetPasswordResetToken(string token);
    Task<bool> MarkTokenAsUsed(TokenType tokenType, Guid tokenId, DateTime usedDate);
    Task<bool> RevokeToken(string refreshToken);
    Task<bool> CreatePasswordResetToken(ResetToken token);
}
