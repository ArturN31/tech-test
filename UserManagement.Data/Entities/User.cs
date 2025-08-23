using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace UserManagement.Data.Models;

public class User : IdentityUser
{
    [Required]
    public string Forename { get; set; } = default!;
    [Required]
    public string Surname { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
