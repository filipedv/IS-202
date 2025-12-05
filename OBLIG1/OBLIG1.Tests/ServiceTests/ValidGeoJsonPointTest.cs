using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using OBLIG1.Models;
using OBLIG1.Services;
using Xunit;

namespace OBLIG1.Tests;


public class CreateAsync_ShouldAcceptValidGeoJsonPointTests
{
// Tester at CreateAsync godtar gyldig GeoJSON Point og lagrer geometry-feltet    
    [Fact]
    public async Task CreateAsync_ShouldAcceptValidGeoJsonPoint()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDb($"GeoJson_ValidPoint_{Guid.NewGuid()}");
        var service = new ObstacleService(db, NullLogger<ObstacleService>.Instance);

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