using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBLIG1.Models
{
    public class Obstacle
    {
        //primærnøkkel for databasen
        public int Id { get; set; }
        //Navn på hinderet, maks lengde 100 tegn
        [Required, StringLength(100)]
        public string Name { get; set; } = "";
        
        //høyde på hinderet, kan være fra 0 til 200
        [Range(0, 200)]
        public double? Height { get; set; }
        
        //Beskrivelse av hinderet, kan være opptil 1000 tegn
        [StringLength(1000)]
        public string? Description { get; set; }
        
        //Angir om hinderet er i kladde modus
        public bool IsDraft { get; set; }

        //ID til brukeren som opprettet hinderet
        [StringLength(256)]
        public string? CreatedByUserId { get; set; }

        // FK til ApplicationUser (tabellen AspNetUsers)
        [ForeignKey(nameof(CreatedByUserId))]
        public ApplicationUser? CreatedByUser { get; set; }

        //Status til hinderet. Kan være Godkjent, Avvist eller pending
        public ObstacleStatus Status { get; set; }

        //Lagrer geometri-data i GeoJson-format
        //Longtext sikrer plass til store mengder tekst
        [Column(TypeName = "longtext")]
        public string? GeometryGeoJson { get; set; }

        //Tidspunk for registrering
        public DateTime RegisteredAt { get; set; }
        
        //hindertype
        public string? Type { get; set; }
    }
}