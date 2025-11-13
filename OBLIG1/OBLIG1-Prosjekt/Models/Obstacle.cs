using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace OBLIG1.Models
{
    public class Obstacle
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        public double? Height { get; set; }   // meter

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Type { get; set; }

        [Column(TypeName = "longtext")]
        public string? GeometryGeoJson { get; set; }

        public DateTime RegisteredAt { get; set; }

        // NYTT: eierskap
        public string? CreatedByUserId { get; set; }
        public IdentityUser? CreatedByUser { get; set; }

        // NYTT: status
        public ObstacleStatus Status { get; set; } = ObstacleStatus.Pending;
    }
}