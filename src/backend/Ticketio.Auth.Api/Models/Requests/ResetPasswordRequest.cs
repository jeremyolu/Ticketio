namespace Ticketio.Auth.Api.Models.Requests;

public class ResetPasswordRequest
{
    public required string Token { get; set; }
    public required string Password { get; set; }
}
