namespace Ticketio.Auth.Api.Models.Requests;

public class RegisterRequest : AuthRequest
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
}
