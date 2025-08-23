using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Models.Users;

public class UserListViewModel
{
    public List<UserListItemViewModel> Items { get; set; } = [];
}

public class UserListItemViewModel
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "Forename is required.")]
    public string Forename { get; set; } = string.Empty;

    [Required(ErrorMessage = "Surname is required.")]
    public string Surname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
