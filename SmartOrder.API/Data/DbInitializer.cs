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
            // Seed Sales Executive
            var salesEmail = "sales@gmail.com";
            var salesUser = await userManager.FindByEmailAsync(salesEmail);

            if (salesUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = salesEmail,
                    Email = salesEmail,
                    FullName = "Sales Executive",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Sales@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "SalesExecutive");
                }
            }

            // Seed Warehouse Manager
            var warehouseEmail = "warehouse@gmail.com";
            var warehouseUser = await userManager.FindByEmailAsync(warehouseEmail);

            if (warehouseUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = warehouseEmail,
                    Email = warehouseEmail,
                    FullName = "Warehouse Manager",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Warehouse@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "WarehouseManager");
                }
            }


        }
    }
}
