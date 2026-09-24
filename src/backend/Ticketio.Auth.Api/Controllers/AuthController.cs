using Microsoft.AspNetCore.Mvc;
using Ticketio.Auth.Api.Interfaces.Services;
using Ticketio.Auth.Api.Models.Requests;
using Ticketio.Core.Api;

namespace Ticketio.Auth.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : BaseController
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        _logger.LogInformation($"Register user request: {request.Email}");

        var response = await _authService.Register(request);
        return SetResponse(response.StatusCode, response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthRequest request)
    {
        _logger.LogInformation($"Login user request: {request.Email}");

        var response = await _authService.Login(request);
        return SetResponse(response.StatusCode, response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(TokenRequest request)
    {
        _logger.LogInformation($"Refresh user request");

        var response = await _authService.Refresh(request);
        return SetResponse(response.StatusCode, response);
    }


    [HttpPost("recovery")]
    public async Task<IActionResult> Recovery(ForgotPasswordRequest request)
    {
        _logger.LogInformation($"Forgot password request");

        var response = await _authService.ForgotPassword(request);
        return SetResponse(response.StatusCode, response);
    }

    [HttpGet("reset/validate")]
    public async Task<IActionResult> ValidateResetToken([FromQuery] string token)
    {
        _logger.LogInformation("Validating password reset token");

        var response = await _authService.ValidateResetToken(token);
        return SetResponse(response.StatusCode, response);
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset([FromBody] ResetPasswordRequest request)
    {
        _logger.LogInformation($"Reset password request");

        var response = await _authService.ResetPassword(request);
        return SetResponse(response.StatusCode, response);
    }
}
