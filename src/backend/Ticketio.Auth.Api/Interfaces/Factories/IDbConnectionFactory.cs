using System.Data;

namespace Ticketio.Auth.Api.Interfaces.Factories;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
