using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Data.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Implementations;
public class LogService(DataContext dataAccess) : ILogService
{
    private readonly DataContext _dataAccess = dataAccess;

    public async Task<int> CountAllLogsAsync()
    {
        var logs = await _dataAccess.GetAllAsync<Log>();
        return logs.Count;
    }

    public async Task<IEnumerable<Log>> GetAllLogsPaginatedAsync(int page, int logsAmount)
    {
        var logs = await _dataAccess.GetAllAsync<Log>();
        var paginatedLogs = logs
            .Skip((page - 1) * logsAmount)
            .Take(logsAmount)
            .ToList();
        return paginatedLogs;
    }

    public async Task<int> CountUsersLogsAsync(string userId, string? filter = null)
    {
        var logs = _dataAccess.GetAll<Log>().Where(log => log.UserId == userId);

        if (!string.IsNullOrEmpty(filter))
        {
            logs = logs.Where(log => log.PerformedAction == filter);
        }

        return await logs.CountAsync();
    }

    public async Task<IEnumerable<Log>> GetUsersLogsPaginatedAsync(string userId, int page, int logsAmount, string? filter = null)
    {
        var logs = _dataAccess.GetAll<Log>().Where(log => log.UserId == userId);

        if (!string.IsNullOrEmpty(filter))
        {
            logs = logs.Where(log => log.PerformedAction == filter);
        }

        return await logs
            .Skip((page - 1) * logsAmount)
            .Take(logsAmount)
            .ToListAsync();
    }

    public async Task<Log?> GetLogByIdAsync(long id)
    {
        return await _dataAccess.GetAll<Log>()
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<int> CountLogsByPerformedActionAsync(string? logsActionFilter)
    {
        var logs = logsActionFilter switch
        {
            "View" => await _dataAccess.GetAll<Log>().Where(l => l.PerformedAction == "View User").ToListAsync(),
            "Add" => await _dataAccess.GetAll<Log>().Where(l => l.PerformedAction == "Add User").ToListAsync(),
            "Edit" => await _dataAccess.GetAll<Log>().Where(l => l.PerformedAction == "Edit User").ToListAsync(),
            "Delete" => await _dataAccess.GetAll<Log>().Where(l => l.PerformedAction == "Delete User").ToListAsync(),
            _ => await _dataAccess.GetAllAsync<Log>()
        };
        return logs.Count;
    }

    public async Task<IEnumerable<Log>> GetLogsByPerformedActionPaginatedAsync(string? logsActionFilter, int page, int logsAmount)
    {
        var logs = logsActionFilter switch
        {
            "View" => await _dataAccess.GetAll<Log>().Where(l => l.PerformedAction == "View User").ToListAsync(),
            "Add" => await _dataAccess.GetAll<Log>().Where(l => l.PerformedAction == "Add User").ToListAsync(),
            "Edit" => await _dataAccess.GetAll<Log>().Where(l => l.PerformedAction == "Edit User").ToListAsync(),
            "Delete" => await _dataAccess.GetAll<Log>().Where(l => l.PerformedAction == "Delete User").ToListAsync(),
            _ => await _dataAccess.GetAllAsync<Log>()
        };
        var paginatedLogs = logs
            .Skip((page - 1) * logsAmount)
            .Take(logsAmount)
            .ToList();
        return paginatedLogs;
    }

    public async Task<Log> AddLogAsync(Log newLog)
    {
        var log = new Log
        {
            PerformedAction = newLog.PerformedAction,
            UserId = newLog.UserId,
            TimeStamp = newLog.TimeStamp
        };
        await _dataAccess.CreateAsync(log);
        return log;
    }
}
