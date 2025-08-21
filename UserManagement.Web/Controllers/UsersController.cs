using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models;
using UserManagement.Web.Models.Logs;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogService _logService;
    public UsersController(IUserService userService, ILogService logService)
    {
        _userService = userService;
        _logService = logService;
    }

    [HttpGet]
    public async Task<ViewResult> List(string? accountActivityFilter)
    {
        // If accountActivityFilter is null or empty, get all users
        // Otherwise, filter users based on the accountActivityFilter value
        IEnumerable<Models.User> users = string.IsNullOrEmpty(accountActivityFilter)
            ? await _userService.GetAllAsync()
            : await _userService.FilterByActiveAsync(accountActivityFilter);

        var items = users.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive,
            DateOfBirth = p.DateOfBirth
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    // View a user
    [HttpGet("view/{id}")]
    public async Task<IActionResult> View(long id, int page = 1, int logsAmount = 10)
    {
        // Fetch the user by id using the user service
        User? requiredUser = await _userService.GetByIdAsync(id);

        if (requiredUser == null)
        {
            // If user not found, return NotFound result
            return NotFound();
        }

        // Log the action of viewing a user
        Log? newLog = new Log
        {
            UserId = requiredUser.Id,
            PerformedAction = "View User",
            TimeStamp = System.DateTime.UtcNow
        };
        await _logService.AddLogAsync(newLog);

        int totalUsersLogsCount = await _logService.CountUsersLogsAsync(requiredUser.Id);
        IEnumerable<Log> paginatedUsersLogs = await _logService.GetUsersLogsPaginatedAsync(requiredUser.Id, page, logsAmount);

        int totalPages = (int)Math.Ceiling((double)totalUsersLogsCount / logsAmount);

        // Create a UserListItemViewModel to display the user's details
        var viewUserModel = new UserListItemViewModel
        {
            Id = requiredUser.Id,
            Forename = requiredUser.Forename,
            Surname = requiredUser.Surname,
            Email = requiredUser.Email,
            IsActive = requiredUser.IsActive,
            DateOfBirth = requiredUser.DateOfBirth
        };

        // Create a PaginationViewModel for the logs
        var paginationViewModel = new PaginationViewModel
        {
            CurrentPage = page,
            TotalPages = totalPages,
            TotalItems = totalUsersLogsCount,
            ItemsPerPage = logsAmount,
            ControllerName = "Users",
            ActionName = "View",
        };

        var viewLogModel = new UserDetailsViewModel
        {
            User = viewUserModel,
            Logs = paginatedUsersLogs.Select(log => new LogListItemViewModel
            {
                Id = log.Id,
                UserId = log.UserId,
                PerformedAction = log.PerformedAction,
                TimeStamp = log.TimeStamp
            }).ToList(),
            Pagination = paginationViewModel
        };

        // Return the view with the user model
        return View(viewLogModel);
    }

    // Add a new user
    [HttpGet("add")]
    public IActionResult Add()
    {
        // Display a view to add a new user.
        return View(new UserListItemViewModel());
    }

    [HttpPost("add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(UserListItemViewModel newUserData)
    {
        // If model state is invalid, return the view with the current user model
        if (!ModelState.IsValid)
        {
            return View(newUserData);
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
        Log newLog = new Log
        {
            UserId = newUser.Id,
            PerformedAction = "Add User",
            TimeStamp = System.DateTime.UtcNow
        };

        await _logService.AddLogAsync(newLog);

        // Redirect to the list of users after adding
        return RedirectToAction(nameof(List));
    }

    // Edit a user
    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(long id)
    {
        // Fetch the user by id using the user service
        User? requiredUser = await _userService.GetByIdAsync(id);

        if (requiredUser == null)
        {
            // If user not found, return NotFound result
            return NotFound();
        }

        // Create a UserListItemViewModel to edit the user's details
        var editUserModel = new UserListItemViewModel
        {
            Id = requiredUser.Id,
            Forename = requiredUser.Forename,
            Surname = requiredUser.Surname,
            Email = requiredUser.Email,
            IsActive = requiredUser.IsActive,
            DateOfBirth = requiredUser.DateOfBirth
        };

        // Display a view to edit a user.
        return View(editUserModel);
    }



    [HttpPost("edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, UserListItemViewModel newUserData)
    {
        // If model state is invalid, return the view with the current user model
        if (!ModelState.IsValid)
        {
            return View(newUserData);
        }

        // Fetch the user by id using the user service
        User? existingUser = await _userService.GetByIdAsync(id);

        if (existingUser == null)
        {
            // If user not found, return NotFound result
            return NotFound();
        }

        // Log the action of editing a user
        Log newLog = new Log
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

        // Redirect to the list of users after editing
        return RedirectToAction(nameof(List));
    }

    [HttpPost("delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        // Fetch the user by id using the user service
        User? existingUser = await _userService.GetByIdAsync(id);

        if (existingUser == null)
        {
            // If user not found, return NotFound result
            return NotFound();
        }

        // Log the action of deleting a user
        Log newLog = new Log
        {
            UserId = existingUser.Id,
            PerformedAction = "Delete User",
            TimeStamp = System.DateTime.UtcNow
        };

        await _logService.AddLogAsync(newLog);

        // Delete the user from the user service
        await _userService.DeleteAsync(existingUser.Id);

        // Redirect to the list of users after deletion
        return RedirectToAction(nameof(List));
    }
}
