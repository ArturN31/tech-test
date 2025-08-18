using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public async Task<IEnumerable<User>> FilterByActiveAsync(string? accountActivityFilter)
    {
        return accountActivityFilter?.ToLower() switch
        {
            "active" => await _dataAccess.GetAll<User>().Where(u => u.IsActive).ToListAsync(),
            "inactive" => await _dataAccess.GetAll<User>().Where(u => !u.IsActive).ToListAsync(),
            _ => await _dataAccess.GetAllAsync<User>()
        };
    }

    /// <summary>
    /// Fetch all users in memory
    /// </summary>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dataAccess.GetAllAsync<User>();
    }
}
