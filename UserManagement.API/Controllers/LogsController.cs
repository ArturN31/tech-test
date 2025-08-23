using Microsoft.AspNetCore.Mvc;
using UserManagement.Data.Models;
using UserManagement.Blazor.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogsController(ILogService logService) : ControllerBase
{
    private readonly ILogService _logService = logService;

    // List all logs or filter by performed action
    [HttpGet] //get /api/logs
    public async Task<IActionResult> LogsList([FromQuery] string? logsActionFilter, [FromQuery] int page = 1, [FromQuery] int logsAmount = 10)
    {
        var totalLogsCount = string.IsNullOrEmpty(logsActionFilter)
            ? await _logService.CountAllLogsAsync()
            : await _logService.CountLogsByPerformedActionAsync(logsActionFilter);

        IEnumerable<Log> paginatedLogs = string.IsNullOrEmpty(logsActionFilter)
            ? await _logService.GetAllLogsPaginatedAsync(page, logsAmount)
            : await _logService.GetLogsByPerformedActionPaginatedAsync(logsActionFilter, page, logsAmount);

        var totalPages = (int)Math.Ceiling((double)totalLogsCount / logsAmount);

        var logs = paginatedLogs.Select(p => new Log
        {
            Id = p.Id,
            UserId = p.UserId,
            PerformedAction = p.PerformedAction,
            TimeStamp = p.TimeStamp
        }).ToList();

        var pagination = new Pagination
        {
            CurrentPage = page,
            TotalPages = totalPages,
            TotalItems = totalLogsCount,
            ItemsPerPage = logsAmount,
        };

        var model = new PaginatedLogs
        {
            Logs = logs,
            Pagination = pagination
        };

        return Ok(model);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> View(long id)
    {
        Log? log = await _logService.GetLogByIdAsync(id);

        if (log == null)
        {
            return NotFound();
        }

        var blazorLogModel = new Log
        {
            Id = log.Id,
            UserId = log.UserId,
            PerformedAction = log.PerformedAction,
            TimeStamp = log.TimeStamp
        };

        return Ok(blazorLogModel);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> UserLogsList(string userId, [FromQuery] string? logsActionFilter, [FromQuery] int page = 1, [FromQuery] int logsAmount = 10)
    {
        var totalLogsCount = await _logService.CountUsersLogsAsync(userId, logsActionFilter);
        var paginatedLogs = await _logService.GetUsersLogsPaginatedAsync(userId, page, logsAmount, logsActionFilter);
        var totalPages = (int)Math.Ceiling((double)totalLogsCount / logsAmount);

        var logs = paginatedLogs.Select(p => new Log
        {
            Id = p.Id,
            UserId = p.UserId,
            PerformedAction = p.PerformedAction,
            TimeStamp = p.TimeStamp
        }).ToList();

        var pagination = new Pagination
        {
            CurrentPage = page,
            TotalPages = totalPages,
            TotalItems = totalLogsCount,
            ItemsPerPage = logsAmount,
        };

        var model = new PaginatedLogs
        {
            Logs = logs,
            Pagination = pagination
        };

        return Ok(model);
    }
}
