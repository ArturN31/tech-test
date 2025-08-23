using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Data.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface ILogService
{
    /// <summary>
    /// Return a count of all logs
    /// </summary>
    /// <returns></returns>
    Task<int> CountAllLogsAsync();

    /// <summary>
    /// Return a paginated list of all logs
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<Log>> GetAllLogsPaginatedAsync(int page, int logsAmount);

    /// <summary>
    /// Return a list of user's logs
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<int> CountUsersLogsAsync(string userId, string? filter = null);

    /// <summary>
    /// Return a paginated list of user's logs
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IEnumerable<Log>> GetUsersLogsPaginatedAsync(string userId, int page, int logsAmount, string? filter = null);

    /// <summary>
    /// Retrieves a log entry by its unique identifier.
    /// </summary>
    /// <param name="id" ></param>
    /// <returns></returns>
    Task<Log?> GetLogByIdAsync(long id);

    /// <summary>
    /// Retrieves logs by performed action.
    /// </summary>
    /// <param name="performedAction"></param>
    /// <returns></returns>
    Task<int> CountLogsByPerformedActionAsync(string? performedAction);

    /// <summary>
    /// Retrieves paginated list of logs by performed action.
    /// </summary>
    /// <param name="performedAction"></param>
    /// <returns></returns>
    Task<IEnumerable<Log>> GetLogsByPerformedActionPaginatedAsync(string? performedAction, int page, int logsAmount);

    /// <summary>
    /// Add a new log entry
    /// </summary>
    /// <param name="performedAction" ></param>
    /// <param name="actionDetails"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Log> AddLogAsync(Log newLog);
}
