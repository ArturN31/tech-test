using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
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
            ];
        }
    }

    /// <summary>
    /// Creates a new, isolated DataContext and seeds it with controlled data.
    /// This ensures each test has a clean database instance.
    /// </summary>
    private static DataContext CreateAndSeedContext()
    {
        // Use a unique GUID for the database name to ensure each test is isolated.
        var databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        var context = new DataContext(options);

        // Seed the database with our controlled test data.
        var hasher = new PasswordHasher<User>();
        context.AddRange(TestData.GetSeededUsers(hasher));
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateAndSeedContext();

        var entity = new User
        {
            Id = Guid.NewGuid().ToString(),
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com",
            NormalizedEmail = "BRANDNEWUSER@EXAMPLE.COM",
            UserName = "brandnewuser@example.com",
            NormalizedUserName = "BRANDNEWUSER@EXAMPLE.COM",
            IsActive = true,
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "P@ssword1")
        };
        await context.CreateAsync(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result
            .Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity, options => options.Excluding(u => u.PasswordHash));
    }

    [Fact]
    public async Task GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateAndSeedContext();
        var entity = context.Users.First(u => u.Id == TestData.PeterLoewId);
        await context.DeleteAsync(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public async Task GetAll_WhenUpdated_MustIncludeUpdatedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateAndSeedContext();
        var entity = context.Users.First(u => u.Id == TestData.PeterLoewId);
        entity.Forename = "Updated Name";
        await context.UpdateAsync(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().Contain(s => s.Forename == "Updated Name");
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_MustReturnAllEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateAndSeedContext();

        // Act: Invokes the method under test with the arranged parameters.
        var result = await context.GetAllAsync<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotBeNullOrEmpty().And.HaveCount(3)
              .And.Contain(s => s.Email == "ploew@example.com")
              .And.Contain(s => s.Email == "bfgates@example.com")
              .And.Contain(s => s.Email == "ctroy@example.com");
    }
}
