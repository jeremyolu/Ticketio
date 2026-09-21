namespace Ticketio.Auth.Api.Models.Data;

public class AuthToken
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
