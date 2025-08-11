using InventoryManagement.Domain;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.DataSeed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            try
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

                string[] roleNames = { "Admin", "User" };

                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                        if (!roleResult.Succeeded)
                        {
                            logger.LogError($"Failed to create role: {roleName}");
                        }
                    }
                }

                var adminUserEmail = "admin@1.com";
                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);

                if (adminUser == null)
                {
                    var newAdminUser = new AppUser
                    {
                        Name = "Admin",
                        Email = adminUserEmail,
                        UserName = adminUserEmail,
                        EmailConfirmed = true
                    };

                    var createAdminUser = await userManager.CreateAsync(newAdminUser, "admin");
                    if (createAdminUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAdminUser, "Admin");
                        logger.LogInformation("Admin user created successfully.");
                    }
                    else
                    {
                        logger.LogError($"Failed to create admin user: {string.Join(", ", createAdminUser.Errors.Select(e => e.Description))}");
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
