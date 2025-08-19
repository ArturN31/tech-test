using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

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
    public async Task<IActionResult> View(long id)
    {
        // Fetch the user by id using the user service
        User? requiredUser = await _userService.GetByIdAsync(id);

        if (requiredUser == null)
        {
            // If user not found, return NotFound result
            return NotFound();
        }

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

        // Return the view with the user model
        return View(viewUserModel);
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

        // Delete the user from the user service
        await _userService.DeleteAsync(existingUser.Id);

        // Redirect to the list of users after deletion
        return RedirectToAction(nameof(List));
    }
}
