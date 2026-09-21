using Ticketio.Auth.Api.Models.Data;
using Ticketio.Auth.Api.Models.Requests;

namespace Ticketio.Auth.Api.Interfaces.Repositories;

public interface IAuthRepository
{
    Task<User?> GetUserById(Guid id);
    Task<User?> GetUserByEmail(string email);
    Task<bool> RegisterUser(RegisterRequest request);
    Task<bool> SaveToken(Token token);
    Task<Token?> GetToken(string refreshToken);
    Task<bool> MarkTokenAsUsed(Guid tokenId);
    Task<bool> RevokeToken(string refreshToken);
}
