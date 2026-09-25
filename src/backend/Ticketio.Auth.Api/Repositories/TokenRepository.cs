using Dapper;
using Ticketio.Auth.Api.Enums;
using Ticketio.Auth.Api.Interfaces.Repositories;
using Ticketio.Auth.Api.Models.Data;
using Ticketio.Core.Interfaces.Factories;

namespace Ticketio.Auth.Api.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TokenRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Token?> GetTokenAsync(TokenType tokenType, string token)
    {
        string sql = string.Empty;

        switch (tokenType)
        {
            case TokenType.Verification:
                sql = "SELECT * FROM Ticketio.VerificationTokens WHERE Token = @token;";
                break;
            case TokenType.Refresh:
                sql = "SELECT * FROM Ticketio.RefreshTokens WHERE Token = @token;";
                break;
            case TokenType.Reset:
                sql = "SELECT * FROM Ticketio.ResetTokens WHERE Token = @token;";
                break;
        }

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Token>(sql, new { token });
    }

    public async Task<bool> SaveTokenAsync(TokenType tokenType, Token token)
    {
        string sql = string.Empty;

        switch (tokenType)
        {
            case TokenType.Verification:
                sql = "INSERT INTO Ticketio.VerificationTokens (UserId, Token, ExpiresDate) VALUES (@userId, @token, @expiresDate);";
                break;
            case TokenType.Refresh:
                sql = "INSERT INTO Ticketio.RefreshTokens (UserId, Token, ExpiresDate) VALUES (@userId, @token, @expiresDate);";
                break;
            case TokenType.Reset:
                sql = "INSERT INTO Ticketio.ResetTokens (UserId, Token, ExpiresDate) VALUES (@userId, @token, @expiresDate);";
                break;
        }

        using var connection = _connectionFactory.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            token.UserId,
            Token = token.Hash,
            token.ExpiresDate
        });

        return rowsAffected > 0;
    }

    public async Task<bool> MarkTokenAsUsedAsync(TokenType tokenType, Guid token, DateTime usedDate)
    {
        string sql = string.Empty;

        switch (tokenType)
        {
            case TokenType.Verification:
                sql = "UPDATE Ticketio.VerificationTokens SET UsedDate = @usedDate WHERE Id = @token;";
                break;
            case TokenType.Refresh:
                sql = "UPDATE Ticketio.RefreshTokens SET UsedDate = @usedDate WHERE Id = @token;";
                break;
            case TokenType.Reset:
                sql = "UPDATE Ticketio.ResetTokens SET UsedDate = @usedDate WHERE Id = @token;";
                break;
        }

        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteAsync(sql, new { token }) > 0;
    }

    public async Task<bool> RevokeTokenAsync(TokenType tokenType, string token)
    {
        string sql = string.Empty;

        switch (tokenType)
        {
            case TokenType.Verification:
                sql = "UPDATE Ticketio.VerificationTokens SET IsRevoked = 1 WHERE Token = @token;";
                break;
            case TokenType.Refresh:
                sql = "UPDATE Ticketio.VerificationTokens SET IsRevoked = 1 WHERE Token = @token;";
                break;
            case TokenType.Reset:
                sql = "UPDATE Ticketio.VerificationTokens SET IsRevoked = 1 WHERE Token = @token;";
                break;
        }

        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteAsync(sql, new { token }) > 0;
    }
}
