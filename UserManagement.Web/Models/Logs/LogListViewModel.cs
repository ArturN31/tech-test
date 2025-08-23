using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Models.Logs;

public class LogListViewModel
{
    public List<LogListItemViewModel> Items { get; set; } = new();
    public PaginationViewModel Pagination { get; set; } = new();
}

public class LogListItemViewModel
{
    [Required]
    public required long Id { get; set; }

    [Required]
    public required string UserId { get; set; }

    [Required]
    public string PerformedAction { get; set; } = string.Empty;

    [Required]
    public string ActionDetails { get; set; } = string.Empty;

    [Required]
    public DateTime TimeStamp { get; set; }
}
