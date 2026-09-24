using System.Data;

namespace Ticketio.Core.Interfaces.Factories;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
