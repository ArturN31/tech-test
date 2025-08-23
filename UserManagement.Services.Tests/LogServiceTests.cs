using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Data.Models;
using UserManagement.Services.Implementations;

namespace UserManagement.Services.Tests;
public class LogServiceTests
{
    // A simple, nested static class to hold consistent GUIDs for testing
    private static class TestData
    {
        public static string PeterLoewId { get; } = Guid.NewGuid().ToString();
        public static string BenjaminFranklinId { get; } = Guid.NewGuid().ToString();
        public static string CastorTroyId { get; } = Guid.NewGuid().ToString();

        public static int AddLogId { get; } = 1;
        public static int EditLogId { get; } = 2;
        public static int DeleteLogId { get; } = 3;
        public static int AnotherAddLogId { get; } = 4;
        public static int AnotherEditLogId { get; } = 5;
        public static int AnotherDeleteLogId { get; } = 6;
        public static int UserLogId { get; } = 7;
        public static int UserLogId2 { get; } = 8;
        public static int UserLogId3 { get; } = 9;

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
                }
            ];
        }

        public static IEnumerable<Log> GetSeededLogs()
        {
            return new List<Log>
            {
                new Log { Id = AddLogId, UserId = PeterLoewId, PerformedAction = "Add User", TimeStamp = DateTime.UtcNow.AddMinutes(-1) },
                new Log { Id = EditLogId, UserId = PeterLoewId, PerformedAction = "Edit User", TimeStamp = DateTime.UtcNow.AddMinutes(-2) },
                new Log { Id = DeleteLogId, UserId = PeterLoewId, PerformedAction = "Delete User", TimeStamp = DateTime.UtcNow.AddMinutes(-3) },
                new Log { Id = AnotherAddLogId, UserId = BenjaminFranklinId, PerformedAction = "Add User", TimeStamp = DateTime.UtcNow.AddMinutes(-4) },
                new Log { Id = AnotherEditLogId, UserId = BenjaminFranklinId, PerformedAction = "Edit User", TimeStamp = DateTime.UtcNow.AddMinutes(-5) },
                new Log { Id = AnotherDeleteLogId, UserId = BenjaminFranklinId, PerformedAction = "Delete User", TimeStamp = DateTime.UtcNow.AddMinutes(-6) },
                new Log { Id = UserLogId, UserId = CastorTroyId, PerformedAction = "Add User", TimeStamp = DateTime.UtcNow.AddMinutes(-7) },
                new Log { Id = UserLogId2, UserId = CastorTroyId, PerformedAction = "Edit User", TimeStamp = DateTime.UtcNow.AddMinutes(-8) },
                new Log { Id = UserLogId3, UserId = CastorTroyId, PerformedAction = "Delete User", TimeStamp = DateTime.UtcNow.AddMinutes(-9) }
            };
        }
    }

    /// <summary>
    /// This method creates a new instance of the LogService with an in-memory database context
    /// and seeds it with controlled data.
    /// </summary>
    private static LogService CreateService(out DataContext context)
    {
        // Use a unique database name for each test to ensure isolation.
        var databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        // Creates a new instance of DataContext using the options defined above.
        context = new DataContext(options);

        // Manual seeding for test isolation.
        var hasher = new PasswordHasher<User>();
        context.AddRange(TestData.GetSeededUsers(hasher));
        context.AddRange(TestData.GetSeededLogs());
        context.SaveChanges();

        return new LogService(context);
    }

    [Fact]
    public async Task CountAllLogsAsync_WhenCalled_MustReturnCorrectCount()
    {
        var service = CreateService(out _);
        var expectedCount = await service.CountAllLogsAsync();
        expectedCount.Should().Be(TestData.GetSeededLogs().Count());
    }

    [Fact]
    public async Task GetAllLogsPaginatedAsync_WhenCalled_MustReturnCorrectlyPaginatedLogs()
    {
        var service = CreateService(out var context);
        var page = 2;
        var logsAmount = 3;
        var expectedLogs = context.GetAll<Log>().OrderByDescending(l => l.TimeStamp)
            .Skip((page - 1) * logsAmount)
            .Take(logsAmount)
            .ToList();
        var result = await service.GetAllLogsPaginatedAsync(page, logsAmount);
        result.Should().BeEquivalentTo(expectedLogs);
    }

    [Fact]
    public async Task CountUsersLogsAsync_WhenCalled_MustReturnCorrectCount()
    {
        var service = CreateService(out var context);
        var userId = TestData.PeterLoewId;
        var result = await service.CountUsersLogsAsync(userId);
        var expectedCount = TestData.GetSeededLogs().Count(l => l.UserId == userId);
        result.Should().Be(expectedCount);
    }

    [Fact]
    public async Task GetUsersLogsPaginatedAsync_WhenCalled_MustReturnCorrectlyPaginatedLogs()
    {
        var service = CreateService(out var context);
        var userId = TestData.CastorTroyId;
        var page = 1;
        var logsAmount = 2;
        var expectedLogs = context.GetAll<Log>()
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.TimeStamp)
            .Skip((page - 1) * logsAmount)
            .Take(logsAmount)
            .ToList();
        var result = await service.GetUsersLogsPaginatedAsync(userId, page, logsAmount);
        result.Should().BeEquivalentTo(expectedLogs);
    }

    [Fact]
    public async Task GetLogByIdAsync_WhenCalled_MustReturnCorrectLog()
    {
        var service = CreateService(out var context);
        var logId = TestData.EditLogId;
        var expectedLog = context.GetAll<Log>().FirstOrDefault(l => l.Id == logId);
        var result = await service.GetLogByIdAsync(logId);
        result.Should().BeEquivalentTo(expectedLog);
    }

    [Theory]
    [InlineData("Add", 3)]
    [InlineData("Edit", 3)]
    [InlineData("Delete", 3)]
    [InlineData("View", 0)]
    [InlineData(null, 9)]
    public async Task CountLogsByPerformedActionAsync_WhenCalled_MustReturnCorrectCount(string logsActionFilter, int expectedCount)
    {
        var service = CreateService(out _);
        var result = await service.CountLogsByPerformedActionAsync(logsActionFilter);
        result.Should().Be(expectedCount);
    }

    [Fact]
    public async Task GetLogsByPerformedActionPaginatedAsync_WhenCalled_MustReturnCorrectlyPaginatedLogs()
    {
        var service = CreateService(out var context);
        var logsActionFilter = "Add User";
        var page = 1;
        var logsAmount = 1;
        var expectedLogs = context.GetAll<Log>()
            .Where(l => l.PerformedAction == logsActionFilter)
            .OrderByDescending(l => l.TimeStamp)
            .Skip((page - 1) * logsAmount)
            .Take(logsAmount)
            .ToList();
        var result = await service.GetLogsByPerformedActionPaginatedAsync(logsActionFilter, page, logsAmount);
        result.Should().BeEquivalentTo(expectedLogs);
    }

    [Fact]
    public async Task AddLogAsync_WhenCalled_MustAddLog()
    {
        var service = CreateService(out var context);
        var newLog = new Log
        {
            UserId = TestData.PeterLoewId,
            PerformedAction = "Test Action",
            TimeStamp = DateTime.UtcNow
        };
        var initialCount = context.GetAll<Log>().Count();
        var addedLog = await service.AddLogAsync(newLog);
        var logs = context.GetAll<Log>();
        logs.Count().Should().Be(initialCount + 1);
        logs.Should().ContainSingle(l => l.Id == addedLog.Id &&
                                        l.UserId == newLog.UserId &&
                                        l.PerformedAction == newLog.PerformedAction);
    }
}
