using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OBLIG1.Models
{
    public class ObstacleEditViewModel
    {
        //ID til hinderet som redigeres
        public int Id { get; set; }
        
        //Navn på hinderet
        //Påkrevd å skrive
        [Required, Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;
        
        //Beskrivelse som brukeren fyller ut
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        //Høyde i fot
        [Display(Name = "Height (feet)")]
        [Range(0, 60000)]
        public int? HeightFt { get; set; }   // vises i skjemaet (ft)
        
        [Display(Name = "Obstacle type")]
        public string? Type { get; set; }    // valgt type (Tower/Crane/...)

        // GeoJSON fra kartet på Edit-siden
        public string? GeometryGeoJson { get; set; }

        // Dropdown for obstacle type
        public IEnumerable<SelectListItem> TypeOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();

        // Status (Pending/Approved/Rejected)
        [Display(Name = "Status")]
        public ObstacleStatus Status { get; set; }

        public IEnumerable<SelectListItem> StatusOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();

        // Om innlogget bruker får lov til å endre status (kun Registerfører)
        public bool CanEditStatus { get; set; }

        // Hvem som registrerte hinderet (f.eks. e-post)
        [Display(Name = "Registered by")]
        public string? CreatedByUser { get; set; }
    }
}