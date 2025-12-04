using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;
using OBLIG1.Models;
using OBLIG1.Services;
using Xunit;

namespace OBLIG1.Tests;

/// <summary>
/// Integrasjonstester for ObstacleService med InMemory database.
/// Tester faktisk interaksjon mellom service og database.
/// </summary>
public class ObstacleServiceIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly ObstacleService _service;

    public ObstacleServiceIntegrationTests()
    {
        // Opprett InMemory database for hver test
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new ApplicationDbContext(options);
        _service = new ObstacleService(_db);
    }

    public void Dispose()
    {
        _db.Dispose();
    }
    

    [Fact]
    public async Task CreateAsync_ShouldSaveObstacleToDatabase()
    {
        // Arrange
        var vm = new ObstacleData
        {
            ObstacleName = "Test Tower",
            ObstacleHeight = 50,
            ObstacleDescription = "A test tower",
            GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.75,59.91]}"
        };
        var userId = "user-123";

        // Act
        var result = await _service.CreateAsync(vm, userId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Test Tower", result.Name);
        Assert.Equal(50, result.Height);
        Assert.Equal(userId, result.CreatedByUserId);
        Assert.Equal(ObstacleStatus.Pending, result.Status);

        // Verifiser at det faktisk er lagret i databasen
        var fromDb = await _db.Obstacles.FindAsync(result.Id);
        Assert.NotNull(fromDb);
        Assert.Equal("Test Tower", fromDb.Name);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyName_ShouldUseDefaultName()
    {
        // Arrange
        var vm = new ObstacleData
        {
            ObstacleName = "",
            ObstacleHeight = 100,
            GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.75,59.91]}"
        };

        // Act
        var result = await _service.CreateAsync(vm, "user-456");

        // Assert
        Assert.Equal("Obstacle", result.Name);
    }

    [Fact]
    public async Task CreateAsync_WithNullName_ShouldUseDefaultName()
    {
        // Arrange
        var vm = new ObstacleData
        {
            ObstacleName = null,
            GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.75,59.91]}"
        };

        // Act
        var result = await _service.CreateAsync(vm, "user-789");

        // Assert
        Assert.Equal("Obstacle", result.Name);
    }

    [Fact]
    public async Task CreateAsync_WithZeroHeight_ShouldStoreNullHeight()
    {
        // Arrange
        var vm = new ObstacleData
        {
            ObstacleName = "Zero Height",
            ObstacleHeight = 0,
            GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.75,59.91]}"
        };

        // Act
        var result = await _service.CreateAsync(vm, "user-000");

        // Assert
        Assert.Null(result.Height);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetRegisteredAtToUtcNow()
    {
        // Arrange
        var beforeCreate = DateTime.UtcNow.AddSeconds(-1);
        var vm = new ObstacleData
        {
            ObstacleName = "Timestamp Test",
            GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.75,59.91]}"
        };

        // Act
        var result = await _service.CreateAsync(vm, "user-time");

        // Assert
        var afterCreate = DateTime.UtcNow.AddSeconds(1);
        Assert.InRange(result.RegisteredAt, beforeCreate, afterCreate);
    }
    

    [Fact]
    public async Task GetOverviewAsync_AsRegistrar_ShouldReturnAllObstacles()
    {
        // Arrange - legg til to hindre fra forskjellige brukere
        _db.Obstacles.AddRange(
            new Obstacle { Name = "Obs1", CreatedByUserId = "user-1" },
            new Obstacle { Name = "Obs2", CreatedByUserId = "user-2" },
            new Obstacle { Name = "Obs3", CreatedByUserId = "user-1" }
        );
        await _db.SaveChangesAsync();

        var registrarUser = CreateUserPrincipal("registrar-id", AppRoles.Registrar);

        // Act
        var result = await _service.GetOverviewAsync(registrarUser);

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetOverviewAsync_AsPilot_ShouldReturnOnlyOwnObstacles()
    {
        // Arrange
        var pilotUserId = "pilot-user-id";
        _db.Obstacles.AddRange(
            new Obstacle { Name = "Mine1", CreatedByUserId = pilotUserId },
            new Obstacle { Name = "Andre", CreatedByUserId = "other-user" },
            new Obstacle { Name = "Mine2", CreatedByUserId = pilotUserId }
        );
        await _db.SaveChangesAsync();

        var pilotUser = CreateUserPrincipal(pilotUserId, AppRoles.Pilot);

        // Act
        var result = await _service.GetOverviewAsync(pilotUser);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, o => Assert.Equal(pilotUserId, o.CreatedByUserId));
    }

    [Fact]
    public async Task GetOverviewAsync_ShouldOrderByRegisteredAtDescending()
    {
        // Arrange
        var now = DateTime.UtcNow;
        _db.Obstacles.AddRange(
            new Obstacle { Name = "Oldest", RegisteredAt = now.AddDays(-2), CreatedByUserId = "u" },
            new Obstacle { Name = "Newest", RegisteredAt = now, CreatedByUserId = "u" },
            new Obstacle { Name = "Middle", RegisteredAt = now.AddDays(-1), CreatedByUserId = "u" }
        );
        await _db.SaveChangesAsync();

        var user = CreateUserPrincipal("u", AppRoles.Registrar);

        // Act
        var result = await _service.GetOverviewAsync(user);

        // Assert
        Assert.Equal("Newest", result[0].Name);
        Assert.Equal("Middle", result[1].Name);
        Assert.Equal("Oldest", result[2].Name);
    }
    

    [Fact]
    public async Task GetEditViewModelAsync_WithValidId_ShouldReturnViewModel()
    {
        // Arrange
        var obstacle = new Obstacle
        {
            Name = "Edit Test",
            Height = 100,
            Description = "Test desc",
            Type = "Tower",
            Status = ObstacleStatus.Pending,
            CreatedByUserId = "owner-user"
        };
        _db.Obstacles.Add(obstacle);
        await _db.SaveChangesAsync();

        var user = CreateUserPrincipal("owner-user", AppRoles.Pilot);

        // Act
        var result = await _service.GetEditViewModelAsync(obstacle.Id, user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(obstacle.Id, result.Id);
        Assert.Equal("Edit Test", result.Name);
        Assert.Equal("Test desc", result.Description);
        Assert.Equal("Tower", result.Type);
        // Height konverteres til fot (100m * 3.28084 ≈ 328 ft)
        Assert.NotNull(result.HeightFt);
        Assert.Equal(328, result.HeightFt.Value);
    }

    [Fact]
    public async Task GetEditViewModelAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var user = CreateUserPrincipal("any-user", AppRoles.Registrar);

        // Act
        var result = await _service.GetEditViewModelAsync(9999, user);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetEditViewModelAsync_PilotAccessingOthersObstacle_ShouldThrowUnauthorized()
    {
        // Arrange
        var obstacle = new Obstacle
        {
            Name = "Not Mine",
            CreatedByUserId = "other-user"
        };
        _db.Obstacles.Add(obstacle);
        await _db.SaveChangesAsync();

        var pilot = CreateUserPrincipal("my-user", AppRoles.Pilot);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.GetEditViewModelAsync(obstacle.Id, pilot));
    }

    [Fact]
    public async Task GetEditViewModelAsync_RegistrarCanAccessAnyObstacle()
    {
        // Arrange
        var obstacle = new Obstacle
        {
            Name = "Someone Elses",
            CreatedByUserId = "some-user"
        };
        _db.Obstacles.Add(obstacle);
        await _db.SaveChangesAsync();

        var registrar = CreateUserPrincipal("registrar-user", AppRoles.Registrar);

        // Act
        var result = await _service.GetEditViewModelAsync(obstacle.Id, registrar);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Someone Elses", result.Name);
    }
    

    [Fact]
    public async Task UpdateAsync_ShouldUpdateObstacleFields()
    {
        // Arrange
        var obstacle = new Obstacle
        {
            Name = "Original",
            Description = "Old desc",
            Height = 50,
            Type = "Crane",
            CreatedByUserId = "owner"
        };
        _db.Obstacles.Add(obstacle);
        await _db.SaveChangesAsync();

        var vm = new ObstacleEditViewModel
        {
            Id = obstacle.Id,
            Name = "Updated",
            Description = "New desc",
            HeightFt = 164, // ~50m
            Type = "Tower"
        };

        var user = CreateUserPrincipal("owner", AppRoles.Pilot);

        // Act
        var result = await _service.UpdateAsync(vm, user);

        // Assert
        Assert.True(result);

        var updated = await _db.Obstacles.FindAsync(obstacle.Id);
        Assert.Equal("Updated", updated!.Name);
        Assert.Equal("New desc", updated.Description);
        Assert.Equal("Tower", updated.Type);
    }

    [Fact]
    public async Task UpdateAsync_OnlyRegistrarCanChangeStatus()
    {
        // Arrange
        var obstacle = new Obstacle
        {
            Name = "Status Test",
            Status = ObstacleStatus.Pending,
            CreatedByUserId = "owner"
        };
        _db.Obstacles.Add(obstacle);
        await _db.SaveChangesAsync();

        // Test som Pilot - skal IKKE kunne endre status
        var pilotVm = new ObstacleEditViewModel
        {
            Id = obstacle.Id,
            Name = "Status Test",
            Status = ObstacleStatus.Approved
        };
        var pilot = CreateUserPrincipal("owner", AppRoles.Pilot);
        await _service.UpdateAsync(pilotVm, pilot);

        var afterPilot = await _db.Obstacles.FindAsync(obstacle.Id);
        Assert.Equal(ObstacleStatus.Pending, afterPilot!.Status); // Skal fortsatt være Pending

        // Test som Registerfører - KAN endre status
        var registrarVm = new ObstacleEditViewModel
        {
            Id = obstacle.Id,
            Name = "Status Test",
            Status = ObstacleStatus.Approved
        };
        var registrar = CreateUserPrincipal("registrar", AppRoles.Registrar);
        await _service.UpdateAsync(registrarVm, registrar);

        var afterRegistrar = await _db.Obstacles.FindAsync(obstacle.Id);
        Assert.Equal(ObstacleStatus.Approved, afterRegistrar!.Status);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var vm = new ObstacleEditViewModel { Id = 9999, Name = "Doesnt Exist" };
        var user = CreateUserPrincipal("any", AppRoles.Registrar);

        // Act
        var result = await _service.UpdateAsync(vm, user);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_PilotUpdatingOthersObstacle_ShouldThrowUnauthorized()
    {
        // Arrange
        var obstacle = new Obstacle
        {
            Name = "Not Mine",
            CreatedByUserId = "other-user"
        };
        _db.Obstacles.Add(obstacle);
        await _db.SaveChangesAsync();

        var vm = new ObstacleEditViewModel { Id = obstacle.Id, Name = "Trying to change" };
        var pilot = CreateUserPrincipal("my-user", AppRoles.Pilot);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.UpdateAsync(vm, pilot));
    }

    [Fact]
    public async Task UpdateAsync_ShouldConvertHeightFeetToMeters()
    {
        // Arrange
        var obstacle = new Obstacle
        {
            Name = "Height Convert",
            Height = 100,
            CreatedByUserId = "owner"
        };
        _db.Obstacles.Add(obstacle);
        await _db.SaveChangesAsync();

        var vm = new ObstacleEditViewModel
        {
            Id = obstacle.Id,
            Name = "Height Convert",
            HeightFt = 328 // 328 ft ≈ 100m
        };

        var user = CreateUserPrincipal("owner", AppRoles.Pilot);

        // Act
        await _service.UpdateAsync(vm, user);

        // Assert
        var updated = await _db.Obstacles.FindAsync(obstacle.Id);
        Assert.NotNull(updated!.Height);
        // 328 / 3.28084 ≈ 99.97m
        Assert.InRange(updated.Height.Value, 99, 101);
    }

    
    private static ClaimsPrincipal CreateUserPrincipal(string userId, string role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Role, role)
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }

  
}

