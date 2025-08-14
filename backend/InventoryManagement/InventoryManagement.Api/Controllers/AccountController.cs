using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Requests;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using InventoryManagement.Domain.Entities;
using System.Security.Claims;

namespace InventoryManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly SignInManager<User> _signInManager;
        private readonly LinkGenerator _linkGenerator;

        public AccountController(IAccountService accountService, SignInManager<User> signInManager, LinkGenerator linkGenerator)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
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

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var firstName = User.FindFirstValue("firstName");
            var lastName = User.FindFirst("lastName")?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            var isBlocked = User.FindFirstValue("isBlocked");

            if (bool.TryParse(isBlocked, out var isBlockedBool) && isBlockedBool)
            {
                return Unauthorized();
            }

            return Ok(new
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Roles = roles
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _accountService.Logout();
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

        [HttpGet("login/google")]
        public IActionResult LoginWithGoogle([FromQuery] string returnUrl)
        {
            var callbackUrl = _linkGenerator.GetPathByName(HttpContext, "GoogleLoginCallback") + $"?returnUrl={returnUrl}";

            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", callbackUrl);

            return Challenge(properties, new[] { "Google" });
        }

        [HttpGet("login/google/callback", Name = "GoogleLoginCallback")]
        public async Task<IActionResult> GoogleLoginCallback([FromQuery] string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            await _accountService.LoginWithGoogleAsync(result.Principal);

            return Redirect(returnUrl);
        }
    }
}
