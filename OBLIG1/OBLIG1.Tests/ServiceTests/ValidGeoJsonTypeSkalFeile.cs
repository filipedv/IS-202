using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using OBLIG1.Models;
using OBLIG1.Services;
using Xunit;

namespace OBLIG1.Tests;

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
            GeometryGeoJson = "{\"coordinates\":[7.99,58.14]}"
        };

        // Act
        var result = await service.CreateAsync(vm, "user-123");

        // Assert
        Assert.Null(result.GeometryGeoJson);
    }
}