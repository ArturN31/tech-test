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
    /// <param name="accountActivityFilter"></param>
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

    /// <summary>
    /// Retrieve user from memory by id
    /// <param name="id"></param>
    /// </summary>
    public async Task<User?> GetByIdAsync(long id)
    {
        return await _dataAccess.GetAll<User>()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Add user to memory
    /// <param name="user"></param>
    /// </summary>
    public async Task<User> AddAsync(User user)
    {
        await _dataAccess.CreateAsync(user);
        return user;
    }

    /// <summary>
    /// Update user data in memory
    /// <param name="user"></param>
    /// </summary>
    public async Task<User> UpdateAsync(User user)
    {
        await _dataAccess.UpdateAsync(user);
        return user;
    }

    /// <summary>
    /// Delete user from memory
    /// <param name="user"></param>
    /// </summary>
    public async Task DeleteAsync(long id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            await _dataAccess.DeleteAsync(user);
        }
    }
}
