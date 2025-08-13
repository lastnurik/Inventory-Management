using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
           _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest();
            }

            await _accountService.RegisterAsync(registerRequest);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _accountService.LoginAsync(loginRequest);

            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = HttpContext.Request.Cookies["REFRESH_TOKEN"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("Refresh token not found.");
            }

            await _accountService.RefreshTokenAsync(refreshToken);

            return Ok();
        }
    }
}
