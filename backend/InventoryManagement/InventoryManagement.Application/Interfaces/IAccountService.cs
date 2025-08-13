using InventoryManagement.Domain.Requests;

namespace InventoryManagement.Application.Interfaces
{
    public interface IAccountService
    {
        Task RegisterAsync(RegisterRequest registerRequest);
        Task LoginAsync(LoginRequest loginRequest);
        Task RefreshTokenAsync(string? refreshToken);
    }
}
