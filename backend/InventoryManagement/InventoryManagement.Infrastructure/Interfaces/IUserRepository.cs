using InventoryManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<IdentityResult> AddAsync(AppUser user, string password);

        Task<IEnumerable<AppUser>> GetAllAsync();

        Task<AppUser> GetByIdAsync(Guid id);

        Task<bool> UpdateAsync(AppUser user);

        Task<bool> DeleteAsync(Guid id);

        Task<AppUser?> FindByEmailAsync(string email);

        Task<bool> CheckPasswordAsync(AppUser user, string password);

        Task<IList<string>> GetUserRolesAsync(AppUser user);

        Task<IdentityResult> SetNewRoleAsync(AppUser user, string role);
    }
}
