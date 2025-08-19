using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
{
    /// <summary>
    /// This method creates a new instance of the DataContext with an in-memory database.
    /// </summary>
    private static DataContext CreateContext()
    {
        // Sets up an in-memory database for testing purposes.
        var databaseName = "UserManagement.Data.DataContext.Tests";
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        var context = new DataContext(options);
        return context;
    }

    [Fact]
    public async Task GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();

        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };
        await context.CreateAsync(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result
            .Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public async Task GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        var entity = context.GetAll<User>().First();
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
        var context = CreateContext();
        var entity = context.GetAll<User>().First();
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
        var context = CreateContext();

        // Act: Invokes the method under test with the arranged parameters.
        var result = await context.GetAllAsync<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotBeNullOrEmpty().And.Contain(s => s.Forename == "Benjamin Franklin").And.Contain(s => s.Surname == "Gates");
    }
}
