using Ticketio.Auth.Api.Enums;
using Ticketio.Auth.Api.Models.Data;

namespace Ticketio.Auth.Api.Interfaces.Repositories;

public interface ITokenRepository
{
    Task<Token?> GetTokenAsync(TokenType tokenType, string token);
    Task<bool> SaveTokenAsync(TokenType tokenType, Token token);
    Task<bool> MarkTokenAsUsedAsync(TokenType tokenType, Guid token, DateTime usedDate);
    Task<bool> RevokeTokenAsync(TokenType tokenType, string token);
}
