using UserManagement.Web.Models.Logs;

namespace UserManagement.Web.Models.Users;

public class UserDetailsViewModel
{
    public UserListItemViewModel User { get; set; } = new ();
    public List<LogListItemViewModel> Logs { get; set; } = new ();
    public PaginationViewModel Pagination { get; set; } = new();
}
