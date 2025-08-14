using InventoryManagement.Domain.Requests;
using System.Security.Claims;

namespace InventoryManagement.Application.Interfaces
{
    public interface IAccountService
    {
        Task RegisterAsync(RegisterRequest registerRequest);

        Task LoginAsync(LoginRequest loginRequest);

        Task RefreshTokenAsync(string? refreshToken);

        Task LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal);

        public void Logout();
    }
}
