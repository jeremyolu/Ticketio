using System.Net;

namespace Ticketio.Auth.Api.Models.Responses;

public class AuthResponse<T>
{
    public required HttpStatusCode StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Result { get; set; }
}
