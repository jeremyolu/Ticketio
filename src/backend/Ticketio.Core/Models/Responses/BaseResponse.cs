using System.Net;

namespace Ticketio.Core.Models.Responses;

public class BaseResponse
{
    public required HttpStatusCode StatusCode { get; set; }
    public string? Message { get; set; }
}
