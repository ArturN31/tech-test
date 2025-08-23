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

public class UserServiceTests
{
    // A simple, nested static class to hold consistent GUIDs for testing
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
    /// This method creates a new instance of the UserService with a new, isolated in-memory database context
    /// and seeds it with controlled data.
    /// </summary>
    private static UserService CreateService(out DataContext context)
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
        context.SaveChanges();

        return new UserService(context);
    }

    //FilterByActiveAsync Tests
    [Theory]
    [InlineData("active")]
    [InlineData("inactive")]
    [InlineData(null)]
    public async Task FilterByActiveAsync_WhenCalled_ReturnsCorrectlyFilteredUsers(string? filter)
    {
        // Arrange: Create the service and context.
        var service = CreateService(out var context);

        // Prepare expected users based on the filter.
        IEnumerable<User> expectedUsers = filter switch
        {
            "active" => [.. context.GetAll<User>().Where(u => u.IsActive)],
            "inactive" => [.. context.GetAll<User>().Where(u => !u.IsActive)],
            _ => await context.GetAllAsync<User>()
        };

        // Act: Call the method to filter users by active state.
        var result = await service.FilterByActiveAsync(filter);

        // Assert: Verify that the result matches the expected users.
        result.Should().BeEquivalentTo(expectedUsers);
    }

    //GetAllAsync Tests
    [Fact]
    public async Task GetAllAsync_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        var service = CreateService(out var context);
        var users = context.GetAll<User>().ToList();
        var result = await service.GetAllAsync();
        result.Should().BeEquivalentTo(users);
    }

    //GetByIdAsync Tests
    [Fact]
    public async Task GetByIdAsync_WhenUserExists_MustReturnUser()
    {
        var service = CreateService(out var context);
        var user = context.Users.First(u => u.Id == TestData.PeterLoewId);
        var result = await service.GetByIdAsync(TestData.PeterLoewId);
        result.Should().BeEquivalentTo(user, options => options.Excluding(u => u.PasswordHash));
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_MustReturnNull()
    {
        var service = CreateService(out _);
        var nonExistentId = Guid.NewGuid().ToString();
        var result = await service.GetByIdAsync(nonExistentId);
        result.Should().BeNull();
    }

    //AddAsync Tests
    [Fact]
    public async Task AddAsync_WhenUserIsAdded_MustReturnSameUser()
    {
        var service = CreateService(out _);
        var user = new User
        {
            Forename = "Test",
            Surname = "User",
            Email = "test@email.com",
            DateOfBirth = new System.DateTime(1990, 1, 1),
            IsActive = true
        };
        var result = await service.AddAsync(user);
        result.Should().BeEquivalentTo(user);
    }

    //UpdateAsync Tests
    [Fact]
    public async Task UpdateAsync_WhenUserIsUpdated_MustReturnUpdatedUser()
    {
        var service = CreateService(out _);
        var user = await service.GetByIdAsync(TestData.PeterLoewId);
        if (user != null)
        {
            user.Forename = "UpdatedForename";
            await service.UpdateAsync(user);
            var updatedUser = await service.GetByIdAsync(TestData.PeterLoewId);
            updatedUser.Should().BeEquivalentTo(user, options => options.Excluding(u => u.PasswordHash));
        }
    }

    //DeleteAsync Tests
    [Fact]
    public async Task DeleteAsync_WhenUserIsDeleted_MustNotReturnUser()
    {
        var service = CreateService(out _);
        var user = await service.GetByIdAsync(TestData.PeterLoewId);
        if (user != null)
        {
            await service.DeleteAsync(user.Id);
            var deletedUser = await service.GetByIdAsync(TestData.PeterLoewId);
            deletedUser.Should().BeNull();
        }
    }
}
