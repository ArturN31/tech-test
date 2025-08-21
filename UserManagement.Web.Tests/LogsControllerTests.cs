using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Implementations;
using UserManagement.Web.Models.Logs;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Web.Tests;
public class LogsControllerTests
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
    [InlineData("Add User")]
    [InlineData("Edit User")]
    [InlineData("Delete User")]
    [InlineData(null)]
    public async Task LogsList_WithoutFilter_ReturnsPaginatedLogs(string logsActionFilter)
    {
        var (userService, logService) = CreateServices(out var context);
        var controller = new LogsController(logService);
        var result = await controller.LogsList(logsActionFilter, 1, 10);

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<LogListViewModel>().Subject;

        var filteredLogs = string.IsNullOrEmpty(logsActionFilter)
            ? await logService.GetAllLogsPaginatedAsync(1, 10)
            : await logService.GetLogsByPerformedActionPaginatedAsync(logsActionFilter, 1, 10);

        model.Items.Should().HaveCount(filteredLogs.Count());
        model.Items.Should().BeEquivalentTo(filteredLogs, options => options.ExcludingMissingMembers());
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
        var (userService, logService) = CreateServices(out var context);
        var controller = new LogsController(logService);
        long logId = 1;
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
        var (userService, logService) = CreateServices(out var context);
        var controller = new LogsController(logService);
        long invalidLogId = 9999;
        var result = await controller.View(invalidLogId);
        result.Should().BeOfType<NotFoundResult>();
    }
}
