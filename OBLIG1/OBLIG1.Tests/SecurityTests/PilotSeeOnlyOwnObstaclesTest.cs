using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using OBLIG1.Models;
using OBLIG1.Services;
using Xunit;

namespace OBLIG1.Tests;

public class GetOverviewAsync_PilotShouldOnlySeeOwnObstaclesTests
{
    [Fact]
    public async Task GetOverviewAsync_PilotShouldOnlySeeOwnObstacles()
    {
        // Opprett en in-memory database med unikt navn for denne testen
        await using var db = TestHelpers.CreateInMemoryDb($"Auth_PilotOwnOnly_{Guid.NewGuid()}");
        
        // Opprett tjenesten som skal testes (ObstacleService)
        var service = new ObstacleService(db, NullLogger<ObstacleService>.Instance);

        db.Obstacles.AddRange(
            new Obstacle 
            { 
                
                Name = "My Obstacle", 
                CreatedByUserId = "pilot-1", 
                GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[0,0]}" 
            },
            new Obstacle 
            { 
                Name = "Other Obstacle", 
                CreatedByUserId = "pilot-2", 
                GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[0,0]}" 
            }
        );
        await db.SaveChangesAsync();

        var pilotUser = TestHelpers.CreateUser("pilot-1", AppRoles.Pilot);
        
        
        var result = await service.GetOverviewAsync(pilotUser);

        
        Assert.Single(result);
        Assert.Equal("My Obstacle", result[0].Name);
    }
}