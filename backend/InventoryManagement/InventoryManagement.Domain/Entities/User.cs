using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public bool IsBlocked { get; set; } = false;

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiresAtUtc { get; set; }

        public static User Create(string email, string firstName, string lastName)
        {
            return new User
            {
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName,
                IsBlocked = false
            };
        }
    }
}
