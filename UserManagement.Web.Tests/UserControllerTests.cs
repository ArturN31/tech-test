using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Services.Implementations;
using UserManagement.Web.Models.Users;
using UserManagement.Web.Controllers;
using System.Collections.Generic;
using UserManagement.Data.Models;
using Microsoft.AspNetCore.Identity;
using UserManagement.Data;

namespace UserManagement.Web.Tests;

public class UserControllerTests
{
    // Nested static class to hold consistent GUIDs for testing
    private static class TestData
    {
        public static string PeterLoewId { get; } = Guid.NewGuid().ToString();
        public static string BenjaminFranklinId { get; } = Guid.NewGuid().ToString();
        public static string CastorTroyId { get; } = Guid.NewGuid().ToString();
        public static string MemphisRainesId { get; } = Guid.NewGuid().ToString();

        public static IEnumerable<User> GetSeededUsers(PasswordHasher<User> hasher)
        {
            return
            [
                new() {
                    Id = PeterLoewId,
                    Forename = "Peter",
                    Surname = "Loew",
                    Email = "ploew@example.com",
                    IsActive = true,
                    NormalizedEmail = "PLOEW@EXAMPLE.COM",
                    UserName = "ploew@example.com",
                    NormalizedUserName = "PLOEW@EXAMPLE.COM",
                    DateOfBirth = new DateTime(2002, 1, 1),
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                },
                new() {
                    Id = BenjaminFranklinId,
                    Forename = "Benjamin Franklin",
                    Surname = "Gates",
                    Email = "bfgates@example.com",
                    IsActive = true,
                    NormalizedEmail = "BFGATES@EXAMPLE.COM",
                    UserName = "bfgates@example.com",
                    NormalizedUserName = "BFGATES@EXAMPLE.COM",
                    DateOfBirth = new DateTime(1992, 2, 20),
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                },
                new() {
                    Id = CastorTroyId,
                    Forename = "Castor",
                    Surname = "Troy",
                    Email = "ctroy@example.com",
                    IsActive = false,
                    NormalizedEmail = "CTROY@EXAMPLE.COM",
                    UserName = "ctroy@example.com",
                    NormalizedUserName = "CTROY@EXAMPLE.COM",
                    DateOfBirth = new DateTime(1998, 12, 10),
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                },
                new() {
                    Id = MemphisRainesId,
                    Forename = "Memphis",
                    Surname = "Raines",
                    Email = "mraines@example.com",
                    IsActive = true,
                    NormalizedEmail = "MRAINES@EXAMPLE.COM",
                    UserName = "mraines@example.com",
                    NormalizedUserName = "MRAINES@EXAMPLE.COM",
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                }
            ];
        }
    }

    /// <summary>
    /// Creates a new instance of the UserService and LogService with a new, isolated in-memory database context
    /// and seeds it with controlled data.
    /// </summary>
    public static (UserService, LogService) CreateServices(out DataContext context)
    {
        // Sets up an in-memory database with a unique name for testing purposes.
        var databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        // Creates a new instance of DataContext using the options defined above.
        context = new DataContext(options);

        // Seed the database with our controlled test data.
        var hasher = new PasswordHasher<User>();
        context.AddRange(TestData.GetSeededUsers(hasher));
        context.SaveChanges();

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
        var (userService, logService) = CreateServices(out var context);
        var controller = new UsersController(userService, logService);
        var result = await controller.List(accountActivityFilter);
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<UserListViewModel>().Subject;
        var activeUsers = await userService.FilterByActiveAsync(accountActivityFilter);
        model.Items.Should().HaveCount(activeUsers.Count());
        model.Items.Should().BeEquivalentTo(activeUsers, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task View_ValidId_ReturnsUserDetails()
    {
        var (userService, logService) = CreateServices(out _);
        var controller = new UsersController(userService, logService);
        var userId = TestData.PeterLoewId;
        var result = await controller.View(userId);
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<UserDetailsViewModel>().Subject;
        model.User.Id.Should().Be(userId);
        model.User.Forename.Should().NotBeNullOrEmpty();
        model.User.Surname.Should().NotBeNullOrEmpty();
        model.User.Email.Should().NotBeNullOrEmpty();
        model.User.DateOfBirth.Should().NotBeNull();
    }

    [Fact]
    public async Task View_InvalidId_ReturnsNotFound()
    {
        var (userService, logService) = CreateServices(out _);
        var controller = new UsersController(userService, logService);
        var invalidUserId = Guid.NewGuid().ToString();
        var result = await controller.View(invalidUserId);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Add_ValidUserModel_MustReturnNewUser()
    {
        var (userService, logService) = CreateServices(out _);
        var controller = new UsersController(userService, logService);
        var newUser = new UserListItemViewModel
        {
            Forename = "Test",
            Surname = "User",
            Email = "test@user.com",
            DateOfBirth = new DateTime(1990, 1, 1),
            IsActive = true
        };
        var result = await controller.Add(newUser);
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("List");
    }

    [Fact]
    public async Task Add_InvalidUserModel_ReturnsViewWithModel()
    {
        var (userService, logService) = CreateServices(out _);
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
        var (userService, logService) = CreateServices(out _);
        var controller = new UsersController(userService, logService);
        var existingUser = await userService.GetByIdAsync(TestData.BenjaminFranklinId);

        if (existingUser != null)
        {
            var editedUser = new UserListItemViewModel
            {
                Id = existingUser.Id,
                Forename = "Updated",
                Surname = existingUser.Surname,
                Email = existingUser.Email!,
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
        var (userService, logService) = CreateServices(out _);
        var controller = new UsersController(userService, logService);
        var existingUser = await userService.GetByIdAsync(TestData.PeterLoewId);
        if (existingUser != null)
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
        var (userService, logService) = CreateServices(out _);
        var controller = new UsersController(userService, logService);
        var nonExistingUserId = Guid.NewGuid().ToString();
        var result = await controller.Edit(nonExistingUserId);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ValidId_ReturnsRedirectToList()
    {
        var (userService, logService) = CreateServices(out _);
        var controller = new UsersController(userService, logService);
        var userId = TestData.PeterLoewId;
        var result = await controller.Delete(userId);
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("List");
        var deletedUser = await userService.GetByIdAsync(userId);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task Delete_InvalidId_ReturnsNotFound()
    {
        var (userService, logService) = CreateServices(out _);
        var controller = new UsersController(userService, logService);
        var invalidUserId = Guid.NewGuid().ToString();
        var result = await controller.Delete(invalidUserId);
        result.Should().BeOfType<NotFoundResult>();
    }
}
