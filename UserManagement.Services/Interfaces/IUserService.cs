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
}
