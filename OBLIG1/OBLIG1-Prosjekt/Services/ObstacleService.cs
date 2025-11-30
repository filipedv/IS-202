using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;
using OBLIG1.Models;

namespace OBLIG1.Services;

public class ObstacleService : IObstacleService
{
    private readonly ApplicationDbContext _db;

    public ObstacleService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Obstacle> CreateAsync(ObstacleData vm, string userId)
    {
        var entity = new Obstacle
        {
            Name            = string.IsNullOrWhiteSpace(vm.ObstacleName) ? "Obstacle" : vm.ObstacleName,
            Height          = (vm.ObstacleHeight is null || vm.ObstacleHeight <= 0) ? null : vm.ObstacleHeight,
            Description     = vm.ObstacleDescription ?? string.Empty,
            Type            = null,
            GeometryGeoJson = vm.GeometryGeoJson,
            RegisteredAt    = DateTime.UtcNow,
            CreatedByUserId = userId,
            Status          = ObstacleStatus.Pending
        };


        _db.Obstacles.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<IReadOnlyList<Obstacle>> GetOverviewAsync(ClaimsPrincipal user)
    {
        IQueryable<Obstacle> q = _db.Obstacles
            .OrderByDescending(o => o.RegisteredAt);

        // Registerfører ser alle, pilot ser bare egne
        if (!user.IsInRole(AppRoles.Registrar))
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            q = q.Where(o => o.CreatedByUserId == userId);
        }

        return await q.ToListAsync();
    }

    public async Task<ObstacleEditViewModel?> GetEditViewModelAsync(int id, ClaimsPrincipal user)
    {
        var e = await _db.Obstacles.FindAsync(id);
        if (e == null)
            return null;

        var isRegistrar = user.IsInRole(AppRoles.Registrar);

        // Pilot kan bare se/redigere egne hindere
        if (!isRegistrar)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (e.CreatedByUserId != userId)
                throw new UnauthorizedAccessException();
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
                ? (int)Math.Round(e.Height.Value * 3.28084)
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
        var e = await _db.Obstacles.FindAsync(vm.Id);
        if (e == null)
            return false;

        var isRegistrar = user.IsInRole(AppRoles.Registrar);

        // Pilot kan bare endre egne hindere
        if (!isRegistrar)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (e.CreatedByUserId != userId)
                throw new UnauthorizedAccessException();
        }

        e.Name        = vm.Name;
        e.Description = vm.Description;
        e.Type        = vm.Type;
        e.Height      = vm.HeightFt.HasValue
            ? (double?)(vm.HeightFt.Value / 3.28084)
            : null;
        e.GeometryGeoJson = vm.GeometryGeoJson;

        // Kun registerfører kan endre status
        if (isRegistrar)
        {
            e.Status = vm.Status;
        }

        await _db.SaveChangesAsync();
        return true;
    }
}
