using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OBLIG1.Models;

namespace OBLIG1.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1) Roller
            string[] roles = { "Pilot", "Registerforer", "Admin" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2) Brukere

            // 2 piloter
            await EnsureUserWithRoleAsync(
                userManager,
                email: "pilot1@example.com",
                password: "Pilot1!",
                role: "Pilot");

            await EnsureUserWithRoleAsync(
                userManager,
                email: "pilot2@example.com",
                password: "Pilot2!",
                role: "Pilot");

            // 1 registerfører
            await EnsureUserWithRoleAsync(
                userManager,
                email: "registerforer@example.com",
                password: "Register1!",
                role: "Registerforer");

            // 1 admin
            await EnsureUserWithRoleAsync(
                userManager,
                email: "admin@example.com",
                password: "Admin1!",
                role: "Admin");
        }

        private static async Task<ApplicationUser> EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Could not create user {email}: {errors}");
                }
            }

            return user;
        }

        private static async Task EnsureUserWithRoleAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string role)
        {
            var user = await EnsureUserAsync(userManager, email, password);

            if (!await userManager.IsInRoleAsync(user, role))
            {
                var result = await userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Could not add user {email} to role {role}: {errors}");
                }
            }
        }
    }
}
