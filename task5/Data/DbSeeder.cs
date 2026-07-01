using CinemaBooking.Models;
using Microsoft.AspNetCore.Identity;

namespace CinemaBooking.Data
{
    public static class DbSeeder
    {
        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        public static async Task SeedRolesAndAdminAsync(IServiceProvider services, IConfiguration config)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Create roles if not exist
            foreach (var role in new[] { AdminRole, CustomerRole })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2. Create default admin if not exist
            var adminEmail = config["AdminUser:Email"] ?? "admin@cinema.com";
            var adminPassword = config["AdminUser:Password"] ?? "Admin@12345";
            var adminFullName = config["AdminUser:FullName"] ?? "System Admin";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = adminFullName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, AdminRole);
                }
            }
            else if (!await userManager.IsInRoleAsync(existingAdmin, AdminRole))
            {
                await userManager.AddToRoleAsync(existingAdmin, AdminRole);
            }
        }
    }
}