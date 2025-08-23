using UserManagement.Data.Models;

namespace UserManagement.Blazor.Models;

public class PaginatedLogs
{
    public IEnumerable<Log> Logs { get; set; } = [];
    public Pagination Pagination { get; set; } = new();
}
