using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OBLIG1.Data;
using OBLIG1.Models;

namespace OBLIG1.Services;

public class ObstacleService : IObstacleService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<ObstacleService> _logger;

    private const double MetersToFeet = 3.28084;

    public ObstacleService(ApplicationDbContext db, ILogger<ObstacleService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Obstacle> CreateAsync(ObstacleData vm, string userId)
    {
        if (vm == null)
            throw new ArgumentNullException(nameof(vm));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(vm.GeometryGeoJson))
            throw new ArgumentException("GeometryGeoJson cannot be null or empty", nameof(vm));

        var entity = new Obstacle
        {
            Name            = string.IsNullOrWhiteSpace(vm.ObstacleName) ? "Obstacle" : vm.ObstacleName,
            // 0 er en gyldig høyde, bare negative verdier skal konverteres til null
            Height          = (vm.ObstacleHeight is null || vm.ObstacleHeight < 0) ? null : vm.ObstacleHeight,
            Description     = vm.ObstacleDescription ?? string.Empty,
            Type            = null,
            GeometryGeoJson = vm.GeometryGeoJson,
            RegisteredAt    = DateTime.UtcNow,
            CreatedByUserId = userId,
            Status          = ObstacleStatus.Pending
        };

        _db.Obstacles.Add(entity);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Created obstacle {ObstacleId} by user {UserId}", entity.Id, userId);
        return entity;
    }

    public async Task<IReadOnlyList<Obstacle>> GetOverviewAsync(ClaimsPrincipal user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        IQueryable<Obstacle> q = _db.Obstacles
            .OrderByDescending(o => o.RegisteredAt);

        // Registerfører ser alle, pilot ser bare egne
        if (!user.IsInRole(AppRoles.Registrar))
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User principal missing NameIdentifier claim");
                throw new UnauthorizedAccessException("User ID not found in claims");
            }
            q = q.Where(o => o.CreatedByUserId == userId);
        }

        var result = await q.ToListAsync();
        _logger.LogDebug("Retrieved {Count} obstacles for user", result.Count);
        return result;
    }

    public async Task<ObstacleEditViewModel?> GetEditViewModelAsync(int id, ClaimsPrincipal user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var e = await _db.Obstacles.FindAsync(id);
        if (e == null)
        {
            _logger.LogWarning("Attempted to get edit view model for non-existent obstacle {ObstacleId}", id);
            return null;
        }

        var isRegistrar = user.IsInRole(AppRoles.Registrar);

        // Pilot kan bare se/redigere egne hindere
        if (!isRegistrar)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User principal missing NameIdentifier claim during get edit view model");
                throw new UnauthorizedAccessException("User ID not found in claims");
            }
            if (e.CreatedByUserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to access obstacle {ObstacleId} owned by {OwnerId}", 
                    userId, id, e.CreatedByUserId);
                throw new UnauthorizedAccessException("You can only edit your own obstacles");
            }
        }

        // Hent e-post til den som opprettet hinderet
        var createdByEmail = await _db.Users
            .Where(u => u.Id == e.CreatedByUserId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync();

        var vm = new ObstacleEditViewModel
        {
            Id          = e.Id,
            Name        = e.Name,
            Description = e.Description,
            HeightFt    = e.Height.HasValue
                ? (int)Math.Round(e.Height.Value * MetersToFeet)
                : (int?)null,
            Type            = e.Type,
            GeometryGeoJson = e.GeometryGeoJson,
            Status          = e.Status,
            CreatedByUser   = createdByEmail ?? "(unknown)"
            // TypeOptions, StatusOptions, CanEditStatus settes i controlleren (UI-spesifikt)
        };

        return vm;
    }

    public async Task<bool> UpdateAsync(ObstacleEditViewModel vm, ClaimsPrincipal user)
    {
        if (vm == null)
            throw new ArgumentNullException(nameof(vm));
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var e = await _db.Obstacles.FindAsync(vm.Id);
        if (e == null)
        {
            _logger.LogWarning("Attempted to update non-existent obstacle {ObstacleId}", vm.Id);
            return false;
        }

        var isRegistrar = user.IsInRole(AppRoles.Registrar);

        // Pilot kan bare endre egne hindere
        if (!isRegistrar)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User principal missing NameIdentifier claim during update");
                throw new UnauthorizedAccessException("User ID not found in claims");
            }
            if (e.CreatedByUserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to update obstacle {ObstacleId} owned by {OwnerId}", 
                    userId, vm.Id, e.CreatedByUserId);
                throw new UnauthorizedAccessException("You can only update your own obstacles");
            }
        }

        e.Name        = vm.Name;
        e.Description = vm.Description;
        e.Type        = vm.Type;
        e.Height      = vm.HeightFt.HasValue
            ? (double?)(vm.HeightFt.Value / MetersToFeet)
            : null;
        e.GeometryGeoJson = vm.GeometryGeoJson;

        // Kun registerfører kan endre status
        if (isRegistrar)
        {
            e.Status = vm.Status;
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Updated obstacle {ObstacleId} by user", vm.Id);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, ClaimsPrincipal user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var e = await _db.Obstacles.FindAsync(id);
        if (e == null)
        {
            _logger.LogWarning("Attempted to delete non-existent obstacle {ObstacleId}", id);
            return false;
        }

        var isRegistrar = user.IsInRole(AppRoles.Registrar);

        // Pilot kan bare slette egne hindere
        if (!isRegistrar)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User principal missing NameIdentifier claim during delete");
                throw new UnauthorizedAccessException("User ID not found in claims");
            }
            if (e.CreatedByUserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to delete obstacle {ObstacleId} owned by {OwnerId}", 
                    userId, id, e.CreatedByUserId);
                throw new UnauthorizedAccessException("You can only delete your own obstacles");
            }
        }

        _db.Obstacles.Remove(e);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Deleted obstacle {ObstacleId} by user", id);
        return true;
    }
}
