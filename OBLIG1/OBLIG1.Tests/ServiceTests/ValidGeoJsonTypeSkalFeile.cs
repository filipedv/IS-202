using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using OBLIG1.Models;
using OBLIG1.Services;
using Xunit;

namespace OBLIG1.Tests;

// Tester at CreateAsync avviser GeoJSON uten 'type'-felt og kaster ArgumentException uten å lagre noe i databasen
public class CreateAsync_ShouldRejectJsonWithoutTypePropertyTests
{
    [Fact]
    public async Task CreateAsync_ShouldRejectJsonWithoutTypeProperty()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDb($"GeoJson_NoType_{Guid.NewGuid()}");
        var service = new ObstacleService(db, NullLogger<ObstacleService>.Instance);

        var vm = new ObstacleData
        {
            ObstacleName = "Test",
            GeometryGeoJson = "{\"coordinates\":[7.99,58.14]}" // Mangler 'type' property
        };

        // Act & Assert
        // Service skal kaste ArgumentException når GeoJSON mangler 'type' property
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(vm, "user-123"));
        
        // Verifiser at ingenting ble lagret i databasen
        var count = await db.Obstacles.CountAsync();
        Assert.Equal(0, count);
    }
}