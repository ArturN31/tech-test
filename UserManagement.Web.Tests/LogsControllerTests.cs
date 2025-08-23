using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Data.Models;
using UserManagement.Services.Implementations;
using UserManagement.Web.Models.Logs;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Web.Tests;
public class LogsControllerTests
{
    // A simple, nested static class to hold consistent GUIDs and Ids for testing
    private static class TestData
    {
        public static string PeterLoewId { get; } = Guid.NewGuid().ToString();
        public static string BenjaminFranklinId { get; } = Guid.NewGuid().ToString();
        public static string CastorTroyId { get; } = Guid.NewGuid().ToString();
        public static string MemphisRainesId { get; } = Guid.NewGuid().ToString();

        public static int AddLogId { get; } = 1;
        public static int EditLogId { get; } = 2;
        public static int DeleteLogId { get; } = 3;
        public static int AnotherAddLogId { get; } = 4;

        public static IEnumerable<User> GetSeededUsers(PasswordHasher<User> hasher)
        {
            return new List<User>
            {
                new User
                {
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
                new User
                {
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
                new User
                {
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
                new User
                {
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
            };
        }

        public static IEnumerable<Log> GetSeededLogs()
        {
            return new List<Log>
            {
                new Log { Id = AddLogId, UserId = PeterLoewId, PerformedAction = "Add User", TimeStamp = DateTime.UtcNow.AddMinutes(-1) },
                new Log { Id = EditLogId, UserId = PeterLoewId, PerformedAction = "Edit User", TimeStamp = DateTime.UtcNow.AddMinutes(-2) },
                new Log { Id = DeleteLogId, UserId = PeterLoewId, PerformedAction = "Delete User", TimeStamp = DateTime.UtcNow.AddMinutes(-3) },
                new Log { Id = AnotherAddLogId, UserId = BenjaminFranklinId, PerformedAction = "Add User", TimeStamp = DateTime.UtcNow.AddMinutes(-4) },
            };
        }
    }

    /// <summary>
    /// Creates a new instance of the UserService and LogService with a new, isolated in-memory database context
    /// and seeds it with controlled data.
    /// </summary>
    public static (UserService, LogService) CreateServices(out DataContext context)
    {
        // Use a unique GUID for the database name to ensure each test is isolated.
        var databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        // Creates a new instance of DataContext using the options defined above.
        context = new DataContext(options);

        // Seed the database with our controlled test data.
        var hasher = new PasswordHasher<User>();
        context.AddRange(TestData.GetSeededUsers(hasher));
        context.AddRange(TestData.GetSeededLogs());
        context.SaveChanges();

        var userService = new UserService(context);
        var logService = new LogService(context);

        return (userService, logService);
    }

    [Theory]
    [InlineData("Add User")]
    [InlineData("Edit User")]
    [InlineData("Delete User")]
    [InlineData(null)]
    public async Task LogsList_WhenCalledWithOrWithoutFilter_ReturnsCorrectlyPaginatedAndFilteredLogs(string logsActionFilter)
    {
        var (_, logService) = CreateServices(out _);
        var controller = new LogsController(logService);
        var result = await controller.LogsList(logsActionFilter, 1, 10);
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<LogListViewModel>().Subject;

        var expectedLogs = string.IsNullOrEmpty(logsActionFilter)
            ? await logService.GetAllLogsPaginatedAsync(1, 10)
            : await logService.GetLogsByPerformedActionPaginatedAsync(logsActionFilter, 1, 10);

        model.Items.Should().HaveCount(expectedLogs.Count());
        model.Items.Should().BeEquivalentTo(expectedLogs, options => options.ExcludingMissingMembers());
        model.Pagination.CurrentPage.Should().Be(1);
        model.Pagination.TotalPages.Should().BeGreaterThan(0);
        model.Pagination.TotalItems.Should().Be(await logService.CountAllLogsAsync());
        model.Pagination.ItemsPerPage.Should().Be(10);
        model.Pagination.ControllerName.Should().Be("Logs");
        model.Pagination.ActionName.Should().Be("LogsList");
    }

    [Fact]
    public async Task View_ValidId_ReturnsLogDetails()
    {
        var (_, logService) = CreateServices(out _);
        var controller = new LogsController(logService);
        long logId = TestData.AddLogId;
        var result = await controller.View(logId);
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<LogListItemViewModel>().Subject;
        var log = await logService.GetLogByIdAsync(logId);

        model.Should().BeEquivalentTo(new LogListItemViewModel
        {
            Id = log!.Id,
            UserId = log.UserId,
            PerformedAction = log.PerformedAction,
            TimeStamp = log.TimeStamp
        });
    }

    [Fact]
    public async Task View_InvalidId_ReturnsNotFound()
    {
        var (_, logService) = CreateServices(out _);
        var controller = new LogsController(logService);
        long invalidLogId = 9999;
        var result = await controller.View(invalidLogId);
        result.Should().BeOfType<NotFoundResult>();
    }
}
