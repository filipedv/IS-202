/*using Xunit;
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
    
    [Fact]
    //feiler meningen
    public async Task CreateAsync_ShouldRejectJsonWithoutTypeProperty()
    {
        // Arrange
        await using var db = CreateInMemoryDb($"GeoJson_NoType_{Guid.NewGuid()}");
        var service = new ObstacleService(db);

        var vm = new ObstacleData
        {
            ObstacleName = "Test",
            GeometryGeoJson = "{\"coordinates\":[7.99,58.14]}"
        };

        // Act
        var result = await service.CreateAsync(vm, "user-123");

        // Assert
        Assert.Null(result.GeometryGeoJson);
    }
    [Fact]
    public async Task GetOverviewAsync_PilotShouldOnlySeeOwnObstacles()
    {
        // Arrange
        await using var db = CreateInMemoryDb($"Auth_PilotOwnOnly_{Guid.NewGuid()}");
        var service = new ObstacleService(db);

        // Add obstacles from different users
        db.Obstacles.AddRange(
            new Obstacle { Name = "My Obstacle", CreatedByUserId = "pilot-1", GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[0,0]}" },
            new Obstacle { Name = "Other Obstacle", CreatedByUserId = "pilot-2", GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[0,0]}" }
        );
        await db.SaveChangesAsync();

        var pilotUser = CreateUser("pilot-1", AppRoles.Pilot);

        // Act
        var result = await service.GetOverviewAsync(pilotUser);

        // Assert
        Assert.Single(result);
        Assert.Equal("My Obstacle", result[0].Name);
    }
    
    [Fact]
    
    public async Task GetEditViewModelAsync_RegistrarCanAccessAnyObstacle()
    {
        // Arrange
        await using var db = CreateInMemoryDb($"Auth_RegistrarAccess_{Guid.NewGuid()}");
        var service = new ObstacleService(db);

        var obstacle = new Obstacle
        {
            Name = "Any Obstacle",
            CreatedByUserId = "some-pilot",
            GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[0,0]}"
        };
        db.Obstacles.Add(obstacle);
        await db.SaveChangesAsync();

        var registrarUser = CreateUser("registrar-1", AppRoles.Registrar);

        // Act
        var result = await service.GetEditViewModelAsync(obstacle.Id, registrarUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Any Obstacle", result.Name);
    }
    
}
*/
