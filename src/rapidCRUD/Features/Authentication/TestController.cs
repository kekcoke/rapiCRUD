using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace rapidCRUD.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class TestController : ControllerBase
{
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult Public()
    {
        Console.WriteLine("[TestController] Public endpoint called");
        return Ok(new { message = "Public endpoint" });
    }

    [HttpGet("protected")]
    [Authorize]  // Using default scheme
    public IActionResult Protected()
    {
        Console.WriteLine($"[TestController] Protected endpoint called by: {User.Identity?.Name ?? "Unknown"}");
        Console.WriteLine($"[TestController] Is Authenticated: {User.Identity?.IsAuthenticated}");
        Console.WriteLine($"[TestController] Auth Type: {User.Identity?.AuthenticationType}");
        
        return Ok(new 
        { 
            message = "Protected endpoint", 
            user = User.Identity?.Name,
            isAuthenticated = User.Identity?.IsAuthenticated,
            claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        });
    }
}