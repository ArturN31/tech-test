using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="accountActivityFilter"></param>
    /// <returns></returns>
    Task<IEnumerable<User>> FilterByActiveAsync(string? accountActivityFilter);

    /// <summary>
    /// Return all users
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<User>> GetAllAsync();

    /// <summary>
    /// Get User By Id
    /// </summary>
    /// <param name="user" ></param>
    /// <returns></returns>
    Task<User?> GetByIdAsync(long id);

    /// <summary>
    /// Add a new user
    /// </summary>
    /// <param name="user" ></param>
    /// <returns></returns>
    Task<User> AddAsync(User user);

    /// <summary>
    /// Edit user
    /// </summary>
    /// <param name="user" ></param>
    /// <returns></returns>
    Task<User> UpdateAsync(User user);

    /// <summary>
    /// Delete user
    /// </summary>
    /// <param name="id" ></param>
    /// <returns></returns>
    Task DeleteAsync(long id);
}
