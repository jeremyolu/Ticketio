using Ticketio.Auth.Api.Interfaces.Repositories;
using Ticketio.Core.Interfaces.Factories;

namespace Ticketio.Auth.Api.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TokenRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
}
