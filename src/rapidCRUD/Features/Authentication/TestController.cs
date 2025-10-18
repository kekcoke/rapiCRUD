using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace rapidCRUD.Features.Authentication;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("protected")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public IActionResult GetProtected()
    {
        var userName = User.Identity?.Name ?? "Authenticated User";
        return Ok(new { message = $"Hello {userName}, your JWT is valid!" });
    }

    [HttpGet("public")]
    public IActionResult GetPublic()
    {
        return Ok(new { message = "Hello, this is a public endpoint!" });
    }
}
