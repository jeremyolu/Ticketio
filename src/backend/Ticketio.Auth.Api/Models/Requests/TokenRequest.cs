namespace Ticketio.Auth.Api.Models.Requests;

public class TokenRequest
{
    public required string RefreshToken { get; set; }
}
