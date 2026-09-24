namespace Ticketio.Core.Models.Responses;

public class ResultResponse<T> : BaseResponse
{
    public T? Result { get; set; }
}
