using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OBLIG1.Models
{
    public class ObstacleEditViewModel
    {
        public int Id { get; set; }

        [Required, Display(Name = "Name")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [StringLength(1000)]
        public string? Description { get; set; }

        // Vises i skjemaet (lagres som meter i entiteten i controlleren)
        [Display(Name = "Height (feet)")]
        [Range(0, 60000)]
        public int? HeightFt { get; set; }

        [Display(Name = "Obstacle type")]
        [StringLength(100)]
        public string? Type { get; set; }

        [Display(Name = "Draft")]
        public bool IsDraft { get; set; }

        // Viktig: samme casing som i entiteten (GeometryGeoJson)
        // Innholdet settes/oppdateres av kartet i Edit.cshtml
        public string? GeometryGeoJson { get; set; }

        // Kilde for dropdown "Type"
        public IEnumerable<SelectListItem> TypeOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}