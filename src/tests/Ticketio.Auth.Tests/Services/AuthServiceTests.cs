using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using Ticketio.Auth.Api.Interfaces.Repositories;
using Ticketio.Auth.Api.Models.Data;
using Ticketio.Auth.Api.Models.Requests;
using Ticketio.Auth.Api.Services;
using Ticketio.Core.Configuration;

namespace Ticketio.Auth.Tests.Services;

[TestFixture]
public class AuthServicetests
{
    private Mock<ILogger<AuthService>> _logger;
    private IOptions<JwtConfig> _jwtConfig;
    private Mock<IAuthRepository> _authRepository;
    private Mock<ITokenRepository> _tokenRepository;
    private AuthService _authService;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<AuthService>>();
        _jwtConfig = Options.Create(new JwtConfig
        {
            Key = "key",
            Issuer = "issuer",
            Audience = "audience",
            AccessTokenExpiryMinutes = 60,
            RefreshTokenExpiryDays = 10
        });
        _authRepository = new Mock<IAuthRepository>();
        _tokenRepository = new Mock<ITokenRepository>();
        _authService = new AuthService(_logger.Object, _jwtConfig, _authRepository.Object, _tokenRepository.Object);
    }

    [Test]
    public async Task Register_RequestIsNull_ReturnsBadRequestResponse()
    {
        // Arrange

        var request = new RegisterRequest
        {
            Email = "",
            Password = "",
            Name = "",
            Surname = ""
        };

        request = null;

        // Act

        var result = await _authService.Register(request);

        // Assert

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Message, Is.EqualTo("Auth request body is null."));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Register_InvalidRequestDetails_ReturnsBadRequestResponse()
    {
        // Arrange

        var request = new RegisterRequest
        {
            Email = "",
            Password = "",
            Name = "John",
            Surname = "Doe"
        };

        // Act

        var result = await _authService.Register(request);

        // Assert

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Message, Does.Contain("Registration details are invalid."));
        Assert.That(result.Message, Does.Contain("Ensure the email address is valid and the password contains at least 8 characters, 1 symbol and 1 digit."));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Register_UserExist_ReturnsConflictResponse()
    {
        // Arrange

        var request = new RegisterRequest
        {
            Email = "john.doe@email.com",
            Password = "QuertP@$$123",
            Name = "John",
            Surname = "Doe"
        };

        var user = new User
        {
            Email = request.Email,
            Password = "Paswword!*",
            Name = "Liam",
            Surname = "Golden",
            Role = "Buyer"
        };

        _authRepository.Setup(x => x.GetUserAsync(request.Email)).ReturnsAsync(user);

        // Act

        var result = await _authService.Register(request);

        // Assert

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Message, Is.EqualTo("User account already exists. Please use another email address."));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [Test]
    public async Task Register_FailedRegisteration_ReturnsInternalServerErrorResponse()
    {
        // Arrange

        var request = new RegisterRequest
        {
            Email = "john.doe@email.com",
            Password = "QuertP@$$123",
            Name = "John",
            Surname = "Doe"
        };

        _authRepository.Setup(x => x.GetUserAsync(request.Email)).ReturnsAsync(() => null);
        _authRepository.Setup(x => x.RegisterUserAsync(request)).ReturnsAsync(false);

        // Act

        var result = await _authService.Register(request);

        // Assert

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Message, Is.EqualTo("User registration failed. Please try again."));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task Register_RegisterationException_ReturnsInternalServerErrorResponse()
    {
        // Arrange

        var request = new RegisterRequest
        {
            Email = "john.doe@email.com",
            Password = "QuertP@$$123",
            Name = "John",
            Surname = "Doe"
        };

        _authRepository.Setup(x => x.GetUserAsync(request.Email)).ReturnsAsync(() => null);
        _authRepository.Setup(x => x.RegisterUserAsync(request)).ThrowsAsync(new Exception("Database failure"));

        // Act

        var result = await _authService.Register(request);

        // Assert

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Message, Is.EqualTo("An error occurred while processing the request."));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task Register_RegisterationSuccessful_ReturnsOKResponse()
    {
        // Arrange

        var request = new RegisterRequest
        {
            Email = "john.doe@email.com",
            Password = "QuertP@$$123",
            Name = "John",
            Surname = "Doe"
        };

        _authRepository.Setup(x => x.GetUserAsync(request.Email)).ReturnsAsync(() => null);
        _authRepository.Setup(x => x.RegisterUserAsync(request)).ReturnsAsync(true);

        // Act

        var result = await _authService.Register(request);

        // Assert

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Message, Is.EqualTo("User account successfully created."));
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

}
