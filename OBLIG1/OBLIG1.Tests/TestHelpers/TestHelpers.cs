using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;

namespace OBLIG1.Tests;

public static class TestHelpers
{
    public static ApplicationDbContext CreateInMemoryDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new ApplicationDbContext(options);
    }

    public static ClaimsPrincipal CreateUser(string userId, string role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, $"{role.ToLower()}@test.com"),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }
}