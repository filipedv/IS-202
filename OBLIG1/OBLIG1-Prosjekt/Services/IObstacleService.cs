using System.Security.Claims;
using OBLIG1.Models;

namespace OBLIG1.Services;

public interface IObstacleService
{
    Task<Obstacle> CreateAsync(ObstacleData vm, string userId);
    Task<IReadOnlyList<Obstacle>> GetOverviewAsync(ClaimsPrincipal user);

    Task<ObstacleEditViewModel?> GetEditViewModelAsync(int id, ClaimsPrincipal user);
    Task<bool> UpdateAsync(ObstacleEditViewModel vm, ClaimsPrincipal user);
    Task<bool> DeleteAsync(int id, ClaimsPrincipal user);
}