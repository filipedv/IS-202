using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OBLIG1.ViewModels.Admin;

public class AdminUserEditViewModel
{
    public string? Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [DataType(DataType.Password)]
    public string? Password { get; set; }   // påkrevd på Create, valgfritt på Edit

    [Required]
    public string Role { get; set; } = "";

    public List<string> AvailableRoles { get; set; } = new();
}