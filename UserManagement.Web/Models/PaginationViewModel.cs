namespace UserManagement.Web.Models;

public class PaginationViewModel
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int ItemsPerPage { get; set; }
    public string ActionName { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
}
