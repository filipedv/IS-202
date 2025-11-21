using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBLIG1.Models
{
    public class Obstacle
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        [Range(0, 200)]
        public double? Height { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsDraft { get; set; }

        [StringLength(256)]
        public string? CreatedByUserId { get; set; }

        // FK til ApplicationUser (tabellen AspNetUsers)
        [ForeignKey(nameof(CreatedByUserId))]
        public ApplicationUser? CreatedByUser { get; set; }

        public ObstacleStatus Status { get; set; }

        [Column(TypeName = "longtext")]
        public string? GeometryGeoJson { get; set; }

        public DateTime RegisteredAt { get; set; }
        
        public string? Type { get; set; }
    }
}