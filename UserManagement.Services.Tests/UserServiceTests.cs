using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Services.Tests;

public class UserServiceTests
{
    /// <summary>
    /// This method creates a new instance of the UserService with an in-memory database context.
    /// </summary>
    /// <param name="context"></param>
    private static UserService CreateService(out DataContext context)
    {
        // Sets up an in-memory database for testing purposes.
        var databaseName = "UserManagement.Data.UserService.Tests";
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        // Creates a new instance of DataContext using the options defined above.
        context = new DataContext(options);
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
            "active" => context.GetAll<User>().Where(u => u.IsActive).ToList(),
            "inactive" => context.GetAll<User>().Where(u => !u.IsActive).ToList(),
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
        var user = context.GetAll<User>().FirstOrDefault(u => u.Id == 1);
        var result = await service.GetByIdAsync(1);
        result.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_MustReturnNull()
    {
        var service = CreateService(out var context);
        var result = await service.GetByIdAsync(20);
        result.Should().BeNull();
    }

    //AddAsync Tests
    [Fact]
    public async Task AddAsync_WhenUserIsAdded_MustReturnSameUser()
    {
        var service = CreateService(out var context);
        var user = new User
        {
            Forename = "Test",
            Surname = "User",
            Email = "test@email.com",
            DateOfBirth = new System.DateOnly(1990,1,1),
            IsActive = true
        };
        var result = await service.AddAsync(user);
        result.Should().BeEquivalentTo(user);
    }

    //UpdateAsync Tests
    [Fact]
    public async Task UpdateAsync_WhenUserIsUpdated_MustReturnUpdatedUser()
    {
        var service = CreateService(out var context);
        var user = await service.GetByIdAsync(1);
        if (user != null)
        {
            user.Forename = "UpdatedForename";
            await service.UpdateAsync(user);
            var updatedUser = await service.GetByIdAsync(1);
            updatedUser.Should().BeEquivalentTo(user);
        }
    }

    //DeleteAsync Tests
    [Fact]
    public async Task DeleteAsync_WhenUserIsDeleted_MustNotReturnUser()
    {
        var service = CreateService(out var context);
        var user = await service.GetByIdAsync(1);
        if (user != null)
        {
            await service.DeleteAsync(user.Id);
            var deletedUser = await service.GetByIdAsync(1);
            deletedUser.Should().BeNull();
        }
    }   
}
