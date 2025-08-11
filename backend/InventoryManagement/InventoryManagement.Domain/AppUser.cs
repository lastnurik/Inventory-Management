using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Domain
{
    public class AppUser : IdentityUser<Guid>
    {
        public string Name { get; set; } = string.Empty;

        public bool IsBlocked { get; set; } = false;
    }
}
