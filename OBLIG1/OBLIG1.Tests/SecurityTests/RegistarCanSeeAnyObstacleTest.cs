using System;
using System.Threading.Tasks;
using Xunit;
using OBLIG1.Models;
using OBLIG1.Services;

namespace OBLIG1.Tests;

public class GetEditViewModelAsync_RegistrarCanAccessAnyObstacleTests
{
    [Fact]
    public async Task GetEditViewModelAsync_RegistrarCanAccessAnyObstacle()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDb($"Auth_RegistrarAccess_{Guid.NewGuid()}");
        var service = new ObstacleService(db);

        var obstacle = new Obstacle
        {
            Name = "Any Obstacle",
            CreatedByUserId = "some-pilot",
            GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[0,0]}"
        };

        db.Obstacles.Add(obstacle);
        await db.SaveChangesAsync();

        var registrarUser = TestHelpers.CreateUser("registrar-1", AppRoles.Registrar);

        // Act
        var result = await service.GetEditViewModelAsync(obstacle.Id, registrarUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Any Obstacle", result.Name);
    }
}