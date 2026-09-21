using Microsoft.AspNetCore.Mvc;
using Ticketio.Auth.Api.Interfaces.Services;
using Ticketio.Auth.Api.Models.Requests;

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
}
