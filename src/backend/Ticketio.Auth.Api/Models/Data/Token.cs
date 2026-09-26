namespace Ticketio.Auth.Api.Models.Data;

public class Token
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Hash { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ExpiresDate { get; set; }
    public DateTime? UsedDate { get; set; }
}
