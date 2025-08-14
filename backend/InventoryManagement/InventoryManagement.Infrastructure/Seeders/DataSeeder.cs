using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Persistance;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Infrastructure.Seeders
{
    public static class DataSeeder
    {
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var dbContext = services.GetRequiredService<AppDbContext>();

                await dbContext.Database.MigrateAsync();

                await SeedRolesAsync(roleManager);

                await SeedAdminUserAsync(userManager, roleManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<AppDbContext>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("User"));
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            var adminUser = await userManager.FindByEmailAsync("admin@1.com");

            if (adminUser == null)
            {
                var user = new User
                {
                    FirstName = "admin",
                    LastName = "admin",
                    Email = "admin@1.com",
                    UserName = "admin@1.com"
                };

                var result = await userManager.CreateAsync(user, "admin");

                if (result.Succeeded)
                {
                    if (await roleManager.RoleExistsAsync("Admin"))
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                }
            }
        }
    }
}
