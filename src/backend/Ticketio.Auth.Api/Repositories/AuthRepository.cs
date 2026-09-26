using Dapper;
using Ticketio.Auth.Api.Interfaces.Repositories;
using Ticketio.Auth.Api.Models.Data;
using Ticketio.Auth.Api.Models.Requests;
using Ticketio.Core.Interfaces.Factories;

namespace Ticketio.Auth.Api.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AuthRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetUserAsync(Guid id)
    {
        var sql = @"SELECT u.*, r.RoleName AS Role 
                  FROM Ticketio.Users u 
                  JOIN Ticketio.Roles r 
                  ON u.RoleId = r.RoleId 
                  WHERE u.UserId = @id;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { id });
    }

    public async Task<User?> GetUserAsync(string email)
    {
        var sql = @"SELECT u.*, r.RoleName AS Role 
                  FROM Ticketio.Users u 
                  JOIN Ticketio.Roles r 
                  ON u.RoleId = r.RoleId 
                  WHERE u.Email = @email;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { email });
    }

    public async Task<bool> RegisterUserAsync(RegisterRequest request)
    {
        var sql = "INSERT INTO Ticketio.Users (Email, Password, Name, Surname) VALUES (@email, @password, @name, @surname);";

        using var connection = _connectionFactory.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            request.Email,
            request.Password,
            request.Name,
            request.Surname
        });

        return rowsAffected > 0;
    }

    public async Task<bool> UpdateUserPasswordAsync(Guid id, string password)
    {
        var sql = "UPDATE Ticketio.Users SET Password = @password WHERE UserId = @id";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteAsync(sql, new { id, password }) > 0;
    }
}

