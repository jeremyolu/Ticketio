using Ticketio.Auth.Api.Models.Data;
using Ticketio.Auth.Api.Models.Requests;

namespace Ticketio.Auth.Api.Interfaces.Repositories;

public interface IAuthRepository
{
    Task<User?> GetUserAsync(Guid id);
    Task<User?> GetUserAsync(string email);
    Task<bool> RegisterUserAsync(RegisterRequest request);
    Task<bool> UpdateUserPasswordAsync(Guid id, string password);
}
