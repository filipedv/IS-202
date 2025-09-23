using System.ComponentModel.DataAnnotations;

namespace OBLIG1.Models;

public class PointInputModel
{
    [Required, StringLength(80)]
    public string Title { get; set; } = string.Empty;

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
}