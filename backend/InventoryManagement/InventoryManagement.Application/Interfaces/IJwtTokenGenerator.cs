using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(AppUser user, string userRole);
    }
}
