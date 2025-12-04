using Xunit;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;
using OBLIG1.Models;
using OBLIG1.Services;
using System.Security.Claims;

namespace OBLIG1.Tests;

public class SecurityTests
{
    private static ApplicationDbContext CreateInMemoryDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    private static ClaimsPrincipal CreateUser(string userId, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, $"{role.ToLower()}@test.com"),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }
    [Fact]
    public async Task CreateAsync_ShouldAcceptValidGeoJsonPoint()
    {
        // Arrange
        await using var db = CreateInMemoryDb($"GeoJson_ValidPoint_{Guid.NewGuid()}");
        var service = new ObstacleService(db);

        var vm = new ObstacleData
        {
            ObstacleName = "Tower",
            GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[7.9956,58.1467]}"
        };

        // Act
        var result = await service.CreateAsync(vm, "user-123");

        // Assert
        Assert.NotNull(result.GeometryGeoJson);
        Assert.Contains("Point", result.GeometryGeoJson);
    }
}