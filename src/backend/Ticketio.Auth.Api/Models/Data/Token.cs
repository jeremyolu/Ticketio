namespace Ticketio.Auth.Api.Models.Data;

public class Token
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string RefreshToken { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime? UsedDate { get; set; }
}
