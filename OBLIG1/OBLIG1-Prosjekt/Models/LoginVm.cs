using System.ComponentModel.DataAnnotations;

namespace OBLIG1.Models;

public class LoginVm
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = "";
}