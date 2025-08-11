using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryManagement.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        public TestController()
        {

        }

        [HttpGet("mee")]
        public IActionResult Me()
        {
            var userClaimsPrincipal = HttpContext.User;

        if (userClaimsPrincipal?.Identity == null || !userClaimsPrincipal.Identity.IsAuthenticated)
        {
            return Unauthorized("User is not authenticated.");
        }

        var userId = userClaimsPrincipal.FindFirstValue("id");

        var userEmail = userClaimsPrincipal.FindFirstValue("email");

        var userName = userClaimsPrincipal.FindFirstValue("name");

        var isBlockedClaimValue = userClaimsPrincipal.FindFirstValue("isBlocked");
        bool isBlocked = bool.TryParse(isBlockedClaimValue, out var blocked) && blocked;

        var userRoles = userClaimsPrincipal.FindAll("roles").Select(c => c.Value).ToList();

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("Required user ID claim not found.");
        }

        return Ok(new
        {
            UserId = userId,
            Email = userEmail,
            Name = userName,
            IsBlocked = isBlocked,
            Roles = userRoles
        });
        }
    }
}
