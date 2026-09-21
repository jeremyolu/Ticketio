using Dapper;
using Ticketio.Auth.Api.Interfaces.Factories;
using Ticketio.Auth.Api.Interfaces.Repositories;
using Ticketio.Auth.Api.Models.Data;
using Ticketio.Auth.Api.Models.Requests;

namespace Ticketio.Auth.Api.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AuthRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetUserById(Guid id)
    {
        var sql = "SELECT * FROM Users WHERE Id = @id;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { id });
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var sql = "SELECT * FROM Users WHERE Email = @email;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { email });
    }

    public async Task<bool> RegisterUser(RegisterRequest request)
    {
        var sql = "INSERT INTO Users (Email, Password, Name, Surname) VALUES (@email, @password, @name, @surname);";

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

    public async Task<bool> SaveToken(Token token)
    {
        var sql = "INSERT INTO Tokens (UserId, ExpiryDate) VALUES (@userId, @expiryDate);";

        using var connection = _connectionFactory.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            token.UserId,
            token.ExpiryDate
        });

        return rowsAffected > 0;
    }

    public async Task<Token?> GetToken(string refreshToken)
    {
        var sql = "SELECT * FROM Tokens WHERE RefreshToken = @refreshToken;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Token>(sql, new { refreshToken });
    }

    public async Task<bool> MarkTokenAsUsed(Guid tokenId)
    {
        var sql = "UPDATE Tokens SET IsUsed = 1 WHERE Id = @tokenId;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteAsync(sql, new { tokenId }) > 0;
    }

    public async Task<bool> RevokeToken(string refreshToken)
    {
        var sql = "UPDATE Tokens SET IsRevoked = 1 WHERE RefreshToken = @refreshToken;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteAsync(sql, new { refreshToken }) > 0;
    }

    public async Task<bool> RevokeAllTokensForUser(int userId)
    {
        var sql = "UPDATE Tokens SET IsRevoked = 1 WHERE IsRevoked = 0 AND UserId = @userId;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteAsync(sql, new { userId }) > 0;
    }
}

