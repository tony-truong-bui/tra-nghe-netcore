using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TraNgheCore.Models;


namespace TraNgheCore
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityModel>>();

            // Ensure roles exist
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Ensure an admin user exists
            string adminEmail = "admin@site.com";
            string adminPassword = "Truong@1501"; // Change this to a secure password

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityModel
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                // Optionally handle errors here
            }
        }
    }
}
