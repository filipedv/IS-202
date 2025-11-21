using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OBLIG1.Models;

namespace OBLIG1.Data
{
    // Statisk klasse som brukes til å forhåndsfylle databasen med roller og brukere
    public static class SeedData
    {
        // Brukes ved oppstart av applikasjonen for å sørge for at roller og standardbrukere finnes
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1) Roller som skal finnes i systemet
            string[] roles = { "Pilot", "Registerforer", "Admin" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2) Brukere (disse vil alltid få "kjent" passord når appen starter)

            await EnsureUserWithRoleAndPasswordAsync(
                userManager,
                email: "pilot1@example.com",
                password: "Pilot1!",
                role: "Pilot");

            await EnsureUserWithRoleAndPasswordAsync(
                userManager,
                email: "pilot2@example.com",
                password: "Pilot2!",
                role: "Pilot");

            await EnsureUserWithRoleAndPasswordAsync(
                userManager,
                email: "registerforer@example.com",
                password: "Register1!",
                role: "Registerforer");

            await EnsureUserWithRoleAndPasswordAsync(
                userManager,
                email: "admin@example.com",
                password: "Admin1!",
                role: "Admin");
        }
        
        // Sørger for at brukeren finnes, har gitt rolle
        // og at passordet er satt til verdien vi oppgir (overskriver evt. gammel).
        private static async Task EnsureUserWithRoleAndPasswordAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string role)
        {
            // Finn eller opprett bruker
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName       = email,
                    Email          = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Could not create user {email}: {errors}");
                }
            }
            else
            {
                // Reset passord
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await userManager.ResetPasswordAsync(user, token, password);
                if (!passResult.Succeeded)
                {
                    var errors = string.Join(", ", passResult.Errors.Select(e => e.Description));
                    throw new Exception($"Could not reset password for {email}: {errors}");
                }
            }

            // Sørg for at brukeren har riktig rolle
            if (!await userManager.IsInRoleAsync(user, role))
            {
                var roleResult = await userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Could not add user {email} to role {role}: {errors}");
                }
            }
        }
    }
}
