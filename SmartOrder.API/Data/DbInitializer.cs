using Microsoft.AspNetCore.Identity;
using SmartOrder.API.Models.Entities;

namespace SmartOrder.API.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles =
            {
                "Admin",
                "Customer",
                "SalesExecutive",
                "WarehouseManager",
                "FinanceOfficer"
            };

            // Seed roles
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed default customer user
            var defaultUserEmail = "customer1@example.com";
            var existingUser = await userManager.FindByEmailAsync(defaultUserEmail);
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = defaultUserEmail,
                    Email = defaultUserEmail,
                    FullName = "Default Customer",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }

            // Seed default admin user
            var adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

        }
    }
}
