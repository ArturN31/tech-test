using Microsoft.AspNetCore.Mvc;
using UserManagement.Data.Models;   
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService, ILogService logService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly ILogService _logService = logService;

    // List all users or filter by account activity
    [HttpGet] //get /api/users
    public async Task<IActionResult> List([FromQuery] string? accountActivityFilter)
    {
        // If accountActivityFilter is null or empty, get all users
        // Otherwise, filter users based on the accountActivityFilter value
        IEnumerable<User> users = string.IsNullOrEmpty(accountActivityFilter)
            ? await _userService.GetAllAsync()
            : await _userService.FilterByActiveAsync(accountActivityFilter);

        var items = users.Select(p => new User
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive,
            DateOfBirth = p.DateOfBirth
        });

        return Ok(items);
    }

    // View a user
    [HttpGet("{id}")] //get /api/users/{id}
    public async Task<IActionResult> GetById(string id)
    {
        // Fetch the user by id using the user service
        var requiredUser = await _userService.GetByIdAsync(id);

        if (requiredUser == null)
        {
            // If user not found, return NotFound result
            return NotFound();
        }

        // Log the action of viewing a user
        var newLog = new Log
        {
            UserId = requiredUser.Id,
            PerformedAction = "View User",
            TimeStamp = System.DateTime.UtcNow
        };
        await _logService.AddLogAsync(newLog);

        // Create a UserListItemViewModel to display the user's details
        var userModel = new User
        {
            Id = requiredUser.Id,
            Forename = requiredUser.Forename,
            Surname = requiredUser.Surname,
            Email = requiredUser.Email,
            IsActive = requiredUser.IsActive,
            DateOfBirth = requiredUser.DateOfBirth
        };

        return Ok(userModel);
    }

    // Add a new user
    [HttpPost] // POST /api/users
    public async Task<IActionResult> Add([FromBody] UserListItemViewModel newUserData)
    {
        // If model state is invalid, return the view with the current user model
        if (!ModelState.IsValid)
        {
            return BadRequest(newUserData);
        }

        // If model state is valid, create a new user and add it to the service
        var newUser = new User
        {
            Forename = newUserData.Forename,
            Surname = newUserData.Surname,
            Email = newUserData.Email,
            IsActive = newUserData.IsActive,
            DateOfBirth = newUserData.DateOfBirth
        };

        await _userService.AddAsync(newUser);

        // Log the action of adding a new user
        var newLog = new Log
        {
            UserId = newUser.Id,
            PerformedAction = "Add User",
            TimeStamp = System.DateTime.UtcNow
        };

        await _logService.AddLogAsync(newLog);

        return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUserData);
    }

    // Edit an existing user
    [HttpPut("{id}")] // PUT /api/users/{id}
    public async Task<IActionResult> Edit(string id, [FromBody] UserListItemViewModel newUserData)
    {
        // If model state is invalid, return the view with the current user model
        if (!ModelState.IsValid)
        {
            return BadRequest(newUserData);
        }

        // Fetch the user by id using the user service
        var existingUser = await _userService.GetByIdAsync(id);

        if (existingUser == null)
        {
            // If user not found, return NotFound result
            return NotFound();
        }

        // Log the action of editing a user
        var newLog = new Log
        {
            UserId = existingUser.Id,
            PerformedAction = "Edit User",
            TimeStamp = System.DateTime.UtcNow
        };

        await _logService.AddLogAsync(newLog);

        // Update the existing user with the new values
        existingUser.Forename = newUserData.Forename;
        existingUser.Surname = newUserData.Surname;
        existingUser.Email = newUserData.Email;
        existingUser.IsActive = newUserData.IsActive;
        existingUser.DateOfBirth = newUserData.DateOfBirth;

        // Update the user in the service
        await _userService.UpdateAsync(existingUser);

        return NoContent();
    }

    // Delete a user
    [HttpDelete("{id}")] // DELETE /api/users/{id}
    public async Task<IActionResult> Delete(string id)
    {
        // Fetch the user by id using the user service
       var existingUser = await _userService.GetByIdAsync(id);

        if (existingUser == null)
        {
            // If user not found, return NotFound result
            return NotFound();
        }

        // Log the action of deleting a user
        var newLog = new Log
        {
            UserId = existingUser.Id,
            PerformedAction = "Delete User",
            TimeStamp = System.DateTime.UtcNow
        };

        await _logService.AddLogAsync(newLog);

        // Delete the user from the user service
        await _userService.DeleteAsync(existingUser.Id);

        return NoContent();
    }
}
