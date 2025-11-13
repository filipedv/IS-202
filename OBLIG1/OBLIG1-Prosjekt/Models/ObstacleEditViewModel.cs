using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OBLIG1.Models
{
    public class ObstacleEditViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "Height (feet)")]
        [Range(0, 60000)]
        public int? HeightFt { get; set; }

        [StringLength(100)]
        public string? Type { get; set; }

        public bool IsDraft { get; set; }

        public string? GeometryGeoJson { get; set; }

        public IEnumerable<SelectListItem> TypeOptions { get; set; } = Enumerable.Empty<SelectListItem>();

        // NYTT – status
        [Display(Name = "Status")]
        public ObstacleStatus Status { get; set; } = ObstacleStatus.Pending;

        public IEnumerable<SelectListItem> StatusOptions { get; set; } = Enumerable.Empty<SelectListItem>();

        // vis kontrollen i viewet kun når registerfører
        public bool CanEditStatus { get; set; }
    }
}