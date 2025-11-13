using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBLIG1.Models
{
    public class Obstacle
    {
        public int Id { get; set; } // PK

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        
        // Lagres i METER i DB. Gjør nullable siden Edit-skjemaet har Height i fot som kan stå tomt.
        [Range(0, 2000)]
        public double? Height { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsDraft { get; set; }

        [StringLength(100)]
        public string? Type { get; set; }

        
        // NB: bruk samme casing som i resten av koden (…GeoJson, ikke …GeoJSON)
        [Column(TypeName = "longtext")]
        public string? GeometryGeoJson { get; set; }

        public DateTime RegisteredAt { get; set; }
    }
}