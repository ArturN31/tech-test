using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Implementations;

namespace UserManagement.Services.Tests;
public class LogServiceTests
{
    /// <summary>
    /// This method creates a new instance of the LogService with an in-memory database context.
    /// </summary>
    /// <param name="context"></param>
    private static LogService CreateService(out DataContext context)
    {
        // Sets up an in-memory database for testing purposes.
        var databaseName = "UserManagement.Data.LogService.Tests";
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        // Creates a new instance of DataContext using the options defined above.
        context = new DataContext(options);
        return new LogService(context);
    }

    [Fact]
    public async Task CountAllLogsAsync_WhenCalled_MustReturnCorrectCount()
    {
        var service = CreateService(out var context);
        var expectedCount = await context.GetAllAsync<Log>();
        var result = await service.CountAllLogsAsync();
        result.Should().Be(expectedCount.Count());
    }

    [Fact]
    public async Task GetAllLogsPaginatedAsync_WhenCalled_MustReturnCorrectlyPaginatedLogs()
    {
        var service = CreateService(out var context);
        var page = 1;
        var logsAmount = 5;
        var expectedLogs = await context.GetAllAsync<Log>();
        var paginatedLogs = expectedLogs
            .Skip((page - 1) * logsAmount)
            .Take(logsAmount)
            .ToList();
        var result = await service.GetAllLogsPaginatedAsync(page, logsAmount);
        result.Should().BeEquivalentTo(paginatedLogs);
    }

    [Fact]
    public async Task CountUsersLogsAsync_WhenCalled_MustReturnCorrectCount()
    {
        var service = CreateService(out var context);
        var userId = 1;
        var expectedCount = await context.GetAllAsync<Log>();
        var usersLogsCount = expectedCount.Count(l => l.UserId == userId);
        var result = await service.CountUsersLogsAsync(userId);
        result.Should().Be(usersLogsCount);
    }

    [Fact]
    public async Task GetUsersLogsPaginatedAsync_WhenCalled_MustReturnCorrectlyPaginatedLogs()
    {
        var service = CreateService(out var context);
        var userId = 1;
        var page = 1;
        var logsAmount = 5;
        var expectedLogs = await context.GetAllAsync<Log>();
        var paginatedLogs = expectedLogs
            .Where(l => l.UserId == userId)
            .Skip((page - 1) * logsAmount)
            .Take(logsAmount)
            .ToList();
        var result = await service.GetUsersLogsPaginatedAsync(userId, page, logsAmount);
        result.Should().BeEquivalentTo(paginatedLogs);
    }

    [Fact]
    public async Task GetLogByIdAsync_WhenCalled_MustReturnCorrectLog()
    {
        var service = CreateService(out var context);
        var logId = 1;
        var expectedLog = await context.GetAll<Log>()
            .FirstOrDefaultAsync(l => l.Id == logId);
        var result = await service.GetLogByIdAsync(logId);
        result.Should().BeEquivalentTo(expectedLog);
    }

    [Fact]
    public async Task CountLogsByPerformedActionAsync_WhenCalled_MustReturnCorrectCount()
    {
        var service = CreateService(out var context);
        var logsActionFilter = "Add";
        var logs = logsActionFilter switch
        {
            "Add" => await context.GetAll<Log>().Where(l => l.PerformedAction == "Add User").ToListAsync(),
            "Edit" => await context.GetAll<Log>().Where(l => l.PerformedAction == "Edit User").ToListAsync(),
            "Delete" => await context.GetAll<Log>().Where(l => l.PerformedAction == "Delete User").ToListAsync(),
            _ => throw new ArgumentException("Invalid action filter")
        };
        var result = await service.CountLogsByPerformedActionAsync(logsActionFilter);
        result.Should().Be(logs.Count());
    }

    [Fact]
    public async Task GetLogsByPerformedActionPaginatedAsync_WhenCalled_MustReturnCorrectlyPaginatedLogs()
    {
        var service = CreateService(out var context);
        var logsActionFilter = "Add";
        var page = 1;
        var logsAmount = 5;
        var expectedLogs = await context.GetAllAsync<Log>();
        var paginatedLogs = expectedLogs
            .Where(l => l.PerformedAction == "Add User")
            .Skip((page - 1) * logsAmount)
            .Take(logsAmount)
            .ToList();
        var result = await service.GetLogsByPerformedActionPaginatedAsync(logsActionFilter, page, logsAmount);
        result.Should().BeEquivalentTo(paginatedLogs);
    }

    [Fact]
    public async Task AddLogAsync_WhenCalled_MustAddLog()
    {
        var service = CreateService(out var context);
        var newLog = new Log
        {
            UserId = 1,
            PerformedAction = "Add User",
            TimeStamp = DateTime.UtcNow
        };
        var addedLog = await service.AddLogAsync(newLog);
        var logs = await context.GetAllAsync<Log>();
        logs.Should().ContainSingle(l => l.Id == addedLog.Id && 
                                         l.UserId == newLog.UserId && 
                                         l.PerformedAction == newLog.PerformedAction && 
                                         l.TimeStamp == newLog.TimeStamp);
    }
}
