using System.Linq;
using System.Threading.Tasks;
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
            IsActive = p.IsActive
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }
}
