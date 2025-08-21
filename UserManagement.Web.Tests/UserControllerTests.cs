using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Implementations;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    /// <summary>
    /// Creates a new instance of the UserService and LogService with an in-memory database context.
    /// </summary>
    public static (UserService, LogService) CreateServices(out DataContext context)
    {
        // Sets up an in-memory database with a unique name for testing purposes.
        var databaseName = "UserManagement.Data.UsersController.Tests";
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        // Creates a new instance of DataContext using the options defined above.
        context = new DataContext(options);
        var userService = new UserService(context);
        var logService = new LogService(context);
        return (userService, logService);
    }

    [Theory]
    [InlineData("active")]
    [InlineData("inactive")]
    [InlineData(null)]
    public async Task List_AccountActivityFilter_ReturnsFilteredUsers(string accountActivityFilter)
    {
        // Arrange: Create a UserService and a UsersController instance
        var (userService, logService) = CreateServices(out var context);
        var controller = new UsersController(userService, logService);

        // Act: Call the List method with the provided accountActivityFilter
        var result = await controller.List(accountActivityFilter);

        // Assert: Check that the result is a ViewResult and contains the expected model
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<UserListViewModel>().Subject;

        // Fetch users based on the accountActivityFilter
        var activeUsers = await userService.FilterByActiveAsync(accountActivityFilter);

        // Verify that the model contains the expected items based on the filter
        model.Items.Should().HaveCount(activeUsers.Count());
        model.Items.Should().BeEquivalentTo(activeUsers, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task View_ValidId_ReturnsUserDetails()
    {
        var (userService, logService) = CreateServices(out var context);
        var controller = new UsersController(userService, logService);
        var userId = 1;
        var result = await controller.View(userId);
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<UserDetailsViewModel>().Subject;

        // Verify that the model contains the expected user details
        model.User.Id.Should().Be(userId);
        model.User.Forename.Should().NotBeNullOrEmpty();
        model.User.Surname.Should().NotBeNullOrEmpty();
        model.User.Email.Should().NotBeNullOrEmpty();
        model.User.DateOfBirth.Should().NotBeNull();
    }

    [Fact]
    public async Task View_InvalidId_ReturnsNotFound()
    {
        var (userService, logService) = CreateServices(out var context);
        var controller = new UsersController(userService, logService);
        var invalidUserId = 20;
        var result = await controller.View(invalidUserId);

        // Assert that the result is a NotFoundResult
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Add_ValidUserModel_MustReturnNewUser()
    {
        var (userService, logService) = CreateServices(out var context);
        var controller = new UsersController(userService, logService);
        var newUser = new UserListItemViewModel
        {
            Forename = "Test",
            Surname = "User",
            Email = "test@user.com",
            DateOfBirth = new DateOnly(1990, 1, 1),
            IsActive = true
        };
        var result = await controller.Add(newUser);

        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("List");
    }

    [Fact]
    public async Task Add_InvalidUserModel_ReturnsViewWithModel()
    {
        var (userService, logService) = CreateServices(out var context);
        var controller = new UsersController(userService, logService);
        controller.ModelState.AddModelError("Forename", "Required");
        var newUser = new UserListItemViewModel
        {
            Surname = "User",
            Email = "", // Missing required fields
            IsActive = true
        };
        var result = await controller.Add(newUser);

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().Be(newUser);
        model.Should().NotBeNull();
    }

    [Fact]
    public async Task Edit_ValidUserModel_MustReturnUpdatedUser()
    {
        var (userService, logService) = CreateServices(out var context);
        var controller = new UsersController(userService, logService);
        var existingUser = await userService.GetByIdAsync(2);

        if(existingUser != null)
        {
            var editedUser = new UserListItemViewModel
            {
                Id = existingUser.Id,
                Forename = "Updated",
                Surname = existingUser.Surname,
                Email = existingUser.Email,
                DateOfBirth = existingUser.DateOfBirth,
                IsActive = existingUser.IsActive
            };
            var result = await controller.Edit(existingUser.Id, editedUser);
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("List");
        }
    }

    [Fact]
    public async Task Edit_InvalidUserModel_ReturnsViewWithModel()
    {
        var (userService, logService) = CreateServices(out var context);
        var controller = new UsersController(userService, logService);
        var existingUser = await userService.GetByIdAsync(1);
        if(existingUser != null)
        {
            controller.ModelState.AddModelError("Forename", "Required");
            var editedUser = new UserListItemViewModel
            {
                Id = existingUser.Id,
                Forename = "", // Missing required fields
                Surname = "", // Missing required fields
                Email = "", // Missing required fields
                DateOfBirth = existingUser.DateOfBirth,
                IsActive = existingUser.IsActive
            };
            var result = await controller.Edit(existingUser.Id, editedUser);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().Be(editedUser);
            model.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task Edit_GetNonExistingUser_ShouldReturnNotFoung()
    {
        var (userService, logService) = CreateServices(out var context);
        var controller = new UsersController(userService, logService);
        var nonExistingUserId = 20;
        var result = await controller.Edit(nonExistingUserId);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ValidId_ReturnsRedirectToList()
    {
        var (userService, logService) = CreateServices(out var context);
        var controller = new UsersController(userService, logService);
        var userId = 1;
        var result = await controller.Delete(userId);

        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("List");
        var deletedUser = await userService.GetByIdAsync(userId);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task Delete_InvalidId_ReturnsNotFound()
    {
        var (userService, logService) = CreateServices(out var context);
        var controller = new UsersController(userService, logService);
        var invalidUserId = 20;
        var result = await controller.Delete(invalidUserId);

        result.Should().BeOfType<NotFoundResult>();
    }
}
