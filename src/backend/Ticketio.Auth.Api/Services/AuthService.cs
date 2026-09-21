using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Ticketio.Auth.Api.Config;
using Ticketio.Auth.Api.Interfaces.Repositories;
using Ticketio.Auth.Api.Interfaces.Services;
using Ticketio.Auth.Api.Models.Data;
using Ticketio.Auth.Api.Models.Requests;
using Ticketio.Auth.Api.Models.Responses;

namespace Ticketio.Auth.Api.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly JwtSettings _jwtSettings;
    private readonly IAuthRepository _authRepository;

    public AuthService(ILogger<AuthService> logger, JwtSettings jwtSettings, IAuthRepository authRepository)
    {
        _logger = logger;
        _jwtSettings = jwtSettings;
        _authRepository = authRepository;
    }

    public async Task<AuthResponse<string>> Register(RegisterRequest request)
    {
        var response = new AuthResponse<string>
        {
            StatusCode = HttpStatusCode.OK
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

            var result = await _authRepository.RegisterUser(request);

            if (!result)
            {
                response.Message = "User registration failed. Please try again.";
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }

            response.Message = "User account successfully created.";
            response.Result = request.Email;
        }
        catch (Exception ex)
        {
            response.Message = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException?.Message : ex.Message;
            response.StatusCode = HttpStatusCode.InternalServerError;
            return response;
        }

        return response;
    }

    public async Task<AuthResponse<AuthToken>> Login(AuthRequest request)
    {
        var response = new AuthResponse<AuthToken>
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
            var user = await _authRepository.GetUserByEmail(request.Email);

            var isAuthenticated = VerifyPassword(request.Password, user?.Password);

            if (user == null || !isAuthenticated)
            {
                response.Message = "Invalid user credentials.";
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }

            response.Result = await GenerateAndSaveTokens(user);
        }
        catch (Exception ex)
        {
            response.Message = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException?.Message : ex.Message;
            response.StatusCode = HttpStatusCode.InternalServerError;
            return response;
        }

        return response;
    }

    public async Task<AuthResponse<AuthToken>> Refresh(TokenRequest request)
    {
        var response = new AuthResponse<AuthToken>
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
            var token = await _authRepository.GetToken(request.RefreshToken);

            if (token == null || token.IsRevoked || token.IsUsed || token.CreatedDate < DateTime.UtcNow)
            {
                _logger.LogWarning($"Refresh token rejected. Exists: {token?.IsRevoked}, Revoked: {token?.IsRevoked}, Used: {token?.IsUsed}, " +
                    $"Expired: {token?.ExpiryDate < DateTime.UtcNow}");

                response.Message = "Invalid or expired refresh token.";
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }

            var user = await _authRepository.GetUserById(token.UserId);

            if (user == null)
            {
                response.Message = "Invalid request.";
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }

            await _authRepository.MarkTokenAsUsed(token.Id);

            response.Result = await GenerateAndSaveTokens(user);
        }
        catch (Exception ex)
        {
            response.Message = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException?.Message : ex.Message;
            response.StatusCode = HttpStatusCode.InternalServerError;
            return response;
        }

        return response;
    }

    public async Task<AuthResponse<string>> ForgotPassword(ForgotPasswordRequest request)
    {
        var response = new AuthResponse<string>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "If an account exists for this email, a password reset link will be sent shortly."
        };

        if (request == null)
        {
            response.Message = "Auth request body is null.";
            response.StatusCode = HttpStatusCode.BadRequest;
            return response;
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            response.Message = "Email has not been provided.";
            response.StatusCode = HttpStatusCode.BadRequest;
            return response;
        }

        try
        {
            var user = await _authRepository.GetUserByEmail(request.Email);

            if (user == null)
                return response;

            var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(GenerateToken())));

            var passwordResetToken = new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                UserId = user.UserId,
                TokenHash = tokenHash,
                CreatedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMinutes(30)
            };

            await _authRepository.CreatePasswordResetToken(passwordResetToken);

            return response;

        }
        catch(Exception ex)
        {
            var errorMessage = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException?.Message : ex.Message;
            _logger.LogCritical(errorMessage);

            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Message = "An error occurred while processing the request.";

            return response;
        }
    }

    private async Task<AuthToken> GenerateAndSaveTokens(User user)
    {
        var refreshToken = GenerateToken();

        var token = new Token
        {
            UserId = user.UserId,
            RefreshToken = refreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };

        await _authRepository.SaveToken(token);

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
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
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
