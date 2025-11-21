using System.ComponentModel.DataAnnotations;

namespace OBLIG1.Models;
public class ObstacleData
{
    //navn på hinderet, max 100 tegn
    [MaxLength(100)]
    public string ObstacleName { get; set; }

    //Påkrevd felt. Må være mellom 0-200
    [Required(ErrorMessage = "Field is required")]
    [Range(0, 200)]
    public double ObstacleHeight { get; set; }

    //beskrivelse av hinderet. Max 1000 tegn
    [MaxLength(1000)]
    public string ObstacleDescription { get; set; }

    //Angir om det er kladd eller ikke
    public bool IsDraft { get; set; }

    
    // Felt som beholder koordinatene til hinderets lokasjon
    public string? GeometryGeoJson { get; set; }
}