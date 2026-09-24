namespace Ticketio.Core.Models.Responses;

public class ResultsResponse<T> : BaseResponse
{
    public IEnumerable<T?> Results { get; set; } = [];
}
