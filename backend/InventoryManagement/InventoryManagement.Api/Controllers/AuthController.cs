using InventoryManagement.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public AuthController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public record CustomRegisterRequest(string Email, string Password, string Name);

        [HttpPost("register2")]
        public async Task<IActionResult> Register(CustomRegisterRequest request)
        {
            var user = new AppUser
            {
                Email = request.Email,
                UserName = request.Email,
                Name = request.Name,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "User");

            return Ok(new { Message = "Registration successful" });
        }
    }
}
