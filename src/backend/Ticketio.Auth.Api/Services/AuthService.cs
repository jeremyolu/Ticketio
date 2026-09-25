using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Ticketio.Auth.Api.Enums;
using Ticketio.Auth.Api.Interfaces.Repositories;
using Ticketio.Auth.Api.Interfaces.Services;
using Ticketio.Auth.Api.Models.Data;
using Ticketio.Auth.Api.Models.Requests;
using Ticketio.Core.Configuration;
using Ticketio.Core.Models.Responses;

namespace Ticketio.Auth.Api.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly JwtConfig _jwtConfig;
    private readonly IAuthRepository _authRepository;
    private readonly ITokenRepository _tokenRepository;

    public AuthService(ILogger<AuthService> logger, IOptions<JwtConfig> options, 
        IAuthRepository authRepository, ITokenRepository tokenRepository)
    {
        _logger = logger;
        _jwtConfig = options.Value;
        _authRepository = authRepository;
        _tokenRepository = tokenRepository;
    }

    public async Task<ResultResponse<string>> Register(RegisterRequest request)
    {
        var response = new ResultResponse<string>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "User account successfully created.",
            Result = request.Email
        };

        if (request == null)
        {
            response.Message = "Auth request body is null.";
            response.StatusCode = HttpStatusCode.BadRequest;
            return response;
        }

        try
        {
            var emailRegex = new Regex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");
            var passwordRegex = new Regex(@"^(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$");

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || 
                !emailRegex.IsMatch(request.Email) || !passwordRegex.IsMatch(request.Password))
                {
                    response.Message = "Registration details are invalid. " +
                        "Ensure the email address is valid and the password contains at least 8 characters, 1 symbol and 1 digit.";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return response;
                }

            request.Password = HashPassword(request.Password);

            var result = await _authRepository.RegisterUserAsync(request);

            if (!result)
            {
                response.Message = "User registration failed. Please try again.";
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }

            return response;
        }
        catch (Exception ex)
        {
            var errorMessage = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException?.Message : ex.Message;
            _logger.LogError(errorMessage);

            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Message = "An error occurred while processing the request.";

            return response;
        }
    }

    public async Task<ResultResponse<AuthToken>> Login(AuthRequest request)
    {
        var response = new ResultResponse<AuthToken>
        {
            StatusCode = HttpStatusCode.OK
        };

        if (request == null)
        {
            response.Message = "Auth request body is null.";
            response.StatusCode = HttpStatusCode.BadRequest;
            return response;
        }

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            response.Message = "Login credentials have not been provided.";
            response.StatusCode = HttpStatusCode.BadRequest;
            return response;
        }

        try
        {
            var user = await _authRepository.GetUserAsync(request.Email);

            var isAuthenticated = VerifyPassword(request.Password, user?.Password);

            if (user == null || !isAuthenticated)
            {
                response.Message = "Invalid user credentials.";
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }

            response.Result = await GenerateAndSaveTokens(TokenType.Refresh, user);

            return response;
        }
        catch (Exception ex)
        {
            var errorMessage = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException?.Message : ex.Message;
            _logger.LogError(errorMessage);

            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Message = "An error occurred while processing the request.";

            return response;
        }
    }

    public async Task<ResultResponse<AuthToken>> Refresh(TokenRequest request)
    {
        var response = new ResultResponse<AuthToken>
        {
            StatusCode = HttpStatusCode.OK
        };

        if (request == null)
        {
            response.Message = "Token request body is null.";
            response.StatusCode = HttpStatusCode.BadRequest;
            return response;
        }

        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            response.Message = "Required tokens have not been provided.";
            response.StatusCode = HttpStatusCode.BadRequest;
            return response;
        }

        try
        {
            var token = await _tokenRepository.GetTokenAsync(TokenType.Refresh, request.RefreshToken);

            if (token == null || token.IsRevoked || token.CreatedDate < DateTime.UtcNow)
            {
                _logger.LogWarning($"Invalid or expired refresh token: {token?.Id}, Revoked: {token?.IsRevoked}, " +
                    $"Expired: {token?.ExpiresDate < DateTime.UtcNow}");

                response.Message = "Invalid request..";
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }

            var user = await _authRepository.GetUserAsync(token.UserId);

            if (user == null)
            {
                response.Message = "Invalid request.";
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }

            await _tokenRepository.MarkTokenAsUsedAsync(TokenType.Refresh, token.Id, DateTime.UtcNow);

            response.Result = await GenerateAndSaveTokens(TokenType.Refresh, user);

            return response; ;
        }
        catch (Exception ex)
        {
            var errorMessage = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException?.Message : ex.Message;
            _logger.LogError(errorMessage);

            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Message = "An error occurred while processing the request.";

            return response;
        }
    }

    public async Task<ResultResponse<bool>> ForgotPassword(ForgotPasswordRequest request)
    {
        var response = new ResultResponse<bool>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "If an account exists for this email, a password reset link will be sent shortly.",
            Result = true
        };

        if (request == null)
        {
            response.Message = "Auth request body is null.";
            response.StatusCode = HttpStatusCode.BadRequest;
            response.Result = false;
            return response;
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            response.Message = "Email has not been provided.";
            response.StatusCode = HttpStatusCode.BadRequest;
            response.Result = false;
            return response;
        }

        try
        {
            var user = await _authRepository.GetUserAsync(request.Email);

            if (user == null)
                return response;

            var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(GenerateToken())));

            var token = new Token
            {
                UserId = user.UserId,
                Hash = tokenHash,
                ExpiresDate = DateTime.UtcNow.AddMinutes(30)
            };

            await _tokenRepository.SaveTokenAsync(TokenType.Reset, token);

            return response;

        }
        catch(Exception ex)
        {
            var errorMessage = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException?.Message : ex.Message;
            _logger.LogError(errorMessage);

            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Message = "An error occurred while processing the request.";
            response.Result = false;

            return response;
        }
    }

    public async Task<ResultResponse<bool>> ValidateResetToken(string token)
    {
        var response = new ResultResponse<bool>
        {
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        if (string.IsNullOrWhiteSpace(token))
        {
            response.Message = "Token has not been provided.";
            response.StatusCode = HttpStatusCode.BadRequest;
            response.Result = false;
            return response;
        }

        try
        {
            var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

            var resetToken = await _tokenRepository.GetTokenAsync(TokenType.Reset, tokenHash);

            if (resetToken == null || resetToken.ExpiresDate < DateTime.UtcNow || resetToken.UsedDate != null)
            {
                response.Message = "Reset token is invalid.";
                response.StatusCode = HttpStatusCode.Unauthorized;
                response.Result = false;
                return response;
            }

            return response;
        }
        catch (Exception ex)
        {
            var errorMessage = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException?.Message : ex.Message;
            _logger.LogError(errorMessage);

            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Message = "An error occurred while processing the request.";
            response.Result = false;

            return response;
        }
    }

    public async Task<ResultResponse<bool>> ResetPassword(ResetPasswordRequest request)
    {
        var response = new ResultResponse<bool>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Passowrd has successfully been reset.",
            Result = true
        };

        try
        {
            var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.Token)));

            var resetToken = await _tokenRepository.GetTokenAsync(TokenType.Reset, tokenHash);

            if (resetToken == null || resetToken.ExpiresDate < DateTime.UtcNow || resetToken.UsedDate != null)
            {
                response.Message = "Reset token is invalid.";
                response.StatusCode = HttpStatusCode.Unauthorized;
                response.Result = false;
                return response;
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                response.Message = "Password has not been provided.";
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Result = false;
                return response;
            }

            if (request.Password.Length < 8)
            {
                response.Message = "Password length must be greater than or equal to 8 characters.";
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Result = false;
                return response;
            }

            var passwordHash = HashPassword(request.Password);

            await _authRepository.UpdateUserPasswordAsync(resetToken.UserId, passwordHash);
            await _tokenRepository.MarkTokenAsUsedAsync(TokenType.Reset, resetToken.Id, DateTime.UtcNow);

            return response;
        }
        catch (Exception ex)
        {
            var errorMessage = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException?.Message : ex.Message;
            _logger.LogError(errorMessage);

            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Message = "An error occurred while processing the request.";
            response.Result = false;

            return response;
        }
    }

    private async Task<AuthToken> GenerateAndSaveTokens(TokenType tokenType, User user)
    {
        var refreshToken = GenerateToken();

        var token = new Token
        {
            UserId = user.UserId,
            Hash = refreshToken,
            ExpiresDate = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryDays)
        };

        await _tokenRepository.SaveTokenAsync(tokenType, token);

        return new AuthToken
        {
            AccessToken = GenerateAccessToken(user),
            RefreshToken = refreshToken
        };
    }

    private string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim("id", user.UserId.ToString()),
            new Claim("email", user.Email),
            new Claim("role", user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    private bool VerifyPassword(string password, string? hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}