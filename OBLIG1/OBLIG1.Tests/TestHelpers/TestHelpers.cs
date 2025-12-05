using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;

namespace OBLIG1.Tests;

public static class TestHelpers
{
    /// Oppretter en ny ApplicationDbContext som bruker en InMemory-database.
    /// Dette gjør det mulig å teste EF Core-logikk uten å bruke en ekte database.
    public static ApplicationDbContext CreateInMemoryDb(string dbName)
    {
        // Konfigurer DbContext til å bruke en InMemoryDatabase
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        // Returner en ny kontekst basert på disse valgene
        return new ApplicationDbContext(options);
    }

    /// Oppretter en ClaimsPrincipal som representerer en innlogget bruker i tester.
    /// Brukes f.eks. i ObstacleService for å teste rolle- og brukerlogikk.
    public static ClaimsPrincipal CreateUser(string userId, string role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, $"{role.ToLower()}@test.com"),
            new(ClaimTypes.Role, role)
        };

        // Lag en identitet basert på claimene
        var identity = new ClaimsIdentity(claims, "TestAuth");
        
        // Pakk identiteten inn i en ClaimsPrincipal (som HttpContext.User bruker)
        return new ClaimsPrincipal(identity);
    }
}