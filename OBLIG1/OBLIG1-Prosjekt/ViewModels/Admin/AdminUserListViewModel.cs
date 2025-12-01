using System.Collections.Generic;

namespace OBLIG1.ViewModels.Admin;

public class AdminUserListViewModel
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public IList<string> Roles { get; set; } = new List<string>();
}