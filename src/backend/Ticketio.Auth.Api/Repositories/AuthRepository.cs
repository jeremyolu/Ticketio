using Dapper;
using Ticketio.Auth.Api.Enums;
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

    public async Task<User?> GetUserById(Guid id)
    {
        var sql = "SELECT * FROM Ticketio.Users WHERE Id = @id;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { id });
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var sql = "SELECT * FROM Ticketio.Users WHERE Email = @email;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { email });
    }

    public async Task<bool> RegisterUser(RegisterRequest request)
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

    public async Task<bool> UpdateUserPassword(Guid id, string password)
    {
        var sql = "UPDATE Ticketio.Users SET Password = @password WHERE UserId = @id";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteAsync(sql, new { id, password }) > 0;
    }

    public async Task<bool> SaveToken(RefreshToken token)
    {
        var sql = "INSERT INTO Ticketio.RefreshTokens (UserId, ExpiryDate) VALUES (@userId, @expiryDate);";

        using var connection = _connectionFactory.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            token.UserId,
            token.ExpiryDate
        });

        return rowsAffected > 0;
    }

    public async Task<RefreshToken?> GetToken(string refreshToken)
    {
        var sql = "SELECT * FROM Ticketio.RefreshTokens WHERE RefreshToken = @refreshToken;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { refreshToken });
    }

    public async Task<ResetToken?> GetPasswordResetToken(string resetToken)
    {
        var sql = "SELECT * FROM Ticketio.ResetTokens WHERE ResetToken = @resetToken;";

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<ResetToken>(sql, new { resetToken });
    }

    public async Task<bool> MarkTokenAsUsed(TokenType tokenType, Guid tokenId, DateTime usedDate)
    {
        var sql = tokenType == TokenType.Refresh ? 
            "UPDATE RefreshTokens SET UsedDate = @usedDate WHERE Id = @tokenId;" :
            "UPDATE ResetTokens SET UsedDate = @usedDate WHERE Id = @tokenId";

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

    public async Task<bool> CreatePasswordResetToken(ResetToken token)
    {
        var sql = "INSERT INTO PasswordResetTokens (UserId, TokenHash, CreatedDate, ExpiryDate) VALUES (@userId, @tokenHash, @createdDate, @expiryDate);";

        using var connection = _connectionFactory.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            token.UserId,
            token.TokenHash,
            token.CreatedDate,
            token.ExpiryDate
        });

        return rowsAffected > 0;
    }
}

