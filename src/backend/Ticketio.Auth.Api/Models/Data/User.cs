namespace Ticketio.Auth.Api.Models.Data;

public class User
{
    public Guid UserId { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public int RoleId { get; set; }
    public DateTime CreatedDate { get; set; }
}