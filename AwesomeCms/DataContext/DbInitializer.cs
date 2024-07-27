using AwesomeCms.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AwesomeCms.DataContext
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.Migrate();

            // Check if the admin role exists
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Check if the regular user role exists
            if (!await roleManager.RoleExistsAsync("RegularUser"))
            {
                await roleManager.CreateAsync(new IdentityRole("RegularUser"));
            }

            // Check if the admin user exists
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser { UserName = "admin", Email = "admin@example.com" };
                await userManager.CreateAsync(adminUser, "Admin@123");

                // Assign admin role to the user
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
