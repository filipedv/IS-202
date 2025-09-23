using System.ComponentModel.DataAnnotations;

namespace OBLIG1.Models;
public class ObstacleData
{
    [MaxLength(100)]
    public string ObstacleName { get; set; }

    [Required(ErrorMessage = "Field is required")]
    [Range(0, 200)]
    public double ObstacleHeight { get; set; }

    [MaxLength(1000)]
    public string ObstacleDescription { get; set; }

    public bool IsDraft { get; set; }

    // Felt som beholder koordinatene til hinderets lokasjon
    public string? GeometryGeoJson { get; set; }
}