using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ticketio.Auth.Api.Controllers;

public class BaseController : ControllerBase
{
    protected IActionResult SetResponse(HttpStatusCode statusCode, object? data = null)
    {
        if (statusCode == HttpStatusCode.NoContent)
            return NoContent();

        return StatusCode((int)statusCode, data);
    }
}
