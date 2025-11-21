using Microsoft.AspNetCore.Identity;

namespace OBLIG1.Models
{
    // Her kan du legge på ekstra felter senere hvis du vil
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; } = "User";
        public bool IsBlocked { get; set; } = false;
    }
}