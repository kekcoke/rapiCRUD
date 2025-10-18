using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace rapidCRUD.Features.Authentication;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProtectedController : ControllerBase
{
    [HttpGet("endpoint")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public IActionResult GetProtectedData()
    {
        var userName = User.Identity?.Name ?? "Authenticated User";
        return Ok(new { message = $"Hello {userName}, your JWT is valid!" });
    }
}
