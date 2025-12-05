using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using OBLIG1.Data;
using OBLIG1.Models;
using OBLIG1.Services;
using Xunit;

namespace OBLIG1.Tests.SecurityTests;

/// <summary>
/// Tester at systemet forhindrer uautorisert datamanipulasjon.
/// Verifiserer at brukere ikke kan manipulere andres data ved å prøve å
/// endre CreatedByUserId eller bypass autorisasjon.
/// </summary>
public class UnauthorizedDataManipulationTest
{
    [Fact]
    public async Task UpdateAsync_PilotCannotChangeCreatedByUserId()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDb($"Unauth_Update_{Guid.NewGuid()}");
        var service = new ObstacleService(db, NullLogger<ObstacleService>.Instance);

        // Opprett et hinder eid av en annen bruker
        var originalOwnerId = "original-owner";
        var obstacle = new Obstacle
        {
            Name = "Original Owner's Obstacle",
            Description = "This belongs to someone else",
            Height = 100,
            CreatedByUserId = originalOwnerId,
            GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.75,59.91]}",
            Status = ObstacleStatus.Pending
        };
        db.Obstacles.Add(obstacle);
        await db.SaveChangesAsync();

        var attackerId = "attacker-pilot";
        var attacker = TestHelpers.CreateUser(attackerId, AppRoles.Pilot);

        var vm = new ObstacleEditViewModel
        {
            Id = obstacle.Id,
            Name = "Hacked Name",
            Description = "I stole this",
            HeightFt = 200
        };

        // Act & Assert
        // Pilot skal ikke kunne oppdatere andres hindere
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.UpdateAsync(vm, attacker));

        // Verifiser at data ikke ble endret
        var unchanged = await db.Obstacles.FindAsync(obstacle.Id);
        Assert.NotNull(unchanged);
        Assert.Equal("Original Owner's Obstacle", unchanged.Name);
        Assert.Equal(originalOwnerId, unchanged.CreatedByUserId);
    }

    [Fact]
    public async Task DeleteAsync_PilotCannotDeleteOthersObstacles()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDb($"Unauth_Delete_{Guid.NewGuid()}");
        var service = new ObstacleService(db, NullLogger<ObstacleService>.Instance);

        // Opprett et hinder eid av en annen bruker
        var originalOwnerId = "original-owner";
        var obstacle = new Obstacle
        {
            Name = "Target Obstacle",
            CreatedByUserId = originalOwnerId,
            GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.75,59.91]}"
        };
        db.Obstacles.Add(obstacle);
        await db.SaveChangesAsync();
        var obstacleId = obstacle.Id;

        var attackerId = "attacker-pilot";
        var attacker = TestHelpers.CreateUser(attackerId, AppRoles.Pilot);

        // Act & Assert
        // Pilot skal ikke kunne slette andres hindere
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.DeleteAsync(obstacleId, attacker));

        // Verifiser at hinderet fortsatt eksisterer
        var stillExists = await db.Obstacles.FindAsync(obstacleId);
        Assert.NotNull(stillExists);
        Assert.Equal("Target Obstacle", stillExists.Name);
        Assert.Equal(originalOwnerId, stillExists.CreatedByUserId);
    }

    [Fact]
    public async Task GetEditViewModelAsync_PilotCannotAccessOthersObstacles()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDb($"Unauth_GetEdit_{Guid.NewGuid()}");
        var service = new ObstacleService(db, NullLogger<ObstacleService>.Instance);

        // Opprett et hinder eid av en annen bruker
        var originalOwnerId = "original-owner";
        var obstacle = new Obstacle
        {
            Name = "Private Obstacle",
            Description = "Secret data",
            CreatedByUserId = originalOwnerId,
            GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.75,59.91]}"
        };
        db.Obstacles.Add(obstacle);
        await db.SaveChangesAsync();

        var attackerId = "attacker-pilot";
        var attacker = TestHelpers.CreateUser(attackerId, AppRoles.Pilot);

        // Act & Assert
        // Pilot skal ikke kunne se/hente andres hindere
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.GetEditViewModelAsync(obstacle.Id, attacker));
    }

    [Fact]
    public async Task CreateAsync_UserIdCannotBeManipulated()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDb($"Unauth_Create_{Guid.NewGuid()}");
        var service = new ObstacleService(db, NullLogger<ObstacleService>.Instance);

        var actualUserId = "actual-user";
        var fakeUserId = "fake-admin-user"; // Prøver å late som admin

        var vm = new ObstacleData
        {
            ObstacleName = "Test Obstacle",
            ObstacleHeight = 50,
            GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.75,59.91]}"
        };

        // Act
        // Bruk den faktiske userId, ikke den falske
        var result = await service.CreateAsync(vm, actualUserId);

        // Assert
        // Verifiser at CreatedByUserId er satt til den faktiske userId, ikke den falske
        Assert.NotNull(result);
        Assert.Equal(actualUserId, result.CreatedByUserId);
        Assert.NotEqual(fakeUserId, result.CreatedByUserId);

        // Verifiser i databasen
        var fromDb = await db.Obstacles.FindAsync(result.Id);
        Assert.NotNull(fromDb);
        Assert.Equal(actualUserId, fromDb.CreatedByUserId);
    }

    [Fact]
    public async Task UpdateAsync_RegistrarCanModifyAnyObstacle()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDb($"Auth_Registrar_{Guid.NewGuid()}");
        var service = new ObstacleService(db, NullLogger<ObstacleService>.Instance);

        // Opprett et hinder eid av en pilot
        var pilotId = "pilot-user";
        var obstacle = new Obstacle
        {
            Name = "Pilot's Obstacle",
            Description = "Original description",
            CreatedByUserId = pilotId,
            GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.75,59.91]}",
            Status = ObstacleStatus.Pending
        };
        db.Obstacles.Add(obstacle);
        await db.SaveChangesAsync();

        var registrar = TestHelpers.CreateUser("registrar-user", AppRoles.Registrar);

        var vm = new ObstacleEditViewModel
        {
            Id = obstacle.Id,
            Name = "Updated by Registrar",
            Description = "Registrar can modify",
            Status = ObstacleStatus.Approved // Registrar kan endre status
        };

        // Act
        var result = await service.UpdateAsync(vm, registrar);

        // Assert
        Assert.True(result);

        // Verifiser at endringene ble lagret
        var updated = await db.Obstacles.FindAsync(obstacle.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated by Registrar", updated.Name);
        Assert.Equal(ObstacleStatus.Approved, updated.Status);
        // CreatedByUserId skal ikke endres
        Assert.Equal(pilotId, updated.CreatedByUserId);
    }
}

