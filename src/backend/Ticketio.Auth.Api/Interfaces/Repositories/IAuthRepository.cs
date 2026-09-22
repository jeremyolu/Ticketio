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
    Task<bool> SaveToken(Token token);
    Task<Token?> GetToken(string refreshToken);
    Task<PasswordResetToken?> GetPasswordResetToken(string token);
    Task<bool> MarkTokenAsUsed(TokenType tokenType, Guid tokenId, DateTime usedDate);
    Task<bool> RevokeToken(string refreshToken);
    Task<bool> CreatePasswordResetToken(PasswordResetToken token);
}
