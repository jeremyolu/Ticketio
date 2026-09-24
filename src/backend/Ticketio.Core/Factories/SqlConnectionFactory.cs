using Microsoft.Data.SqlClient;
using System.Data;
using Ticketio.Core.Interfaces.Factories;

namespace Ticketio.Core.Factories;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connString;

    public SqlConnectionFactory(string connString)
    {
        _connString = connString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connString);
    }
}
