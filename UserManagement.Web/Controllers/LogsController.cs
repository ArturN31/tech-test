using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models;
using UserManagement.Web.Models.Logs;

namespace UserManagement.WebMS.Controllers;

[Route("logs")]
public class LogsController : Controller
{
    private readonly ILogService _logService;
    public LogsController(ILogService logService) => _logService = logService;

    [HttpGet]
    public async Task<ViewResult> LogsList(string? logsActionFilter, int page = 1, int logsAmount = 10)
    {
        int totalLogsCount = string.IsNullOrEmpty(logsActionFilter)
            ? await _logService.CountAllLogsAsync()
            : await _logService.CountLogsByPerformedActionAsync(logsActionFilter);

        IEnumerable<Log> paginatedLogs = string.IsNullOrEmpty(logsActionFilter)
            ? await _logService.GetAllLogsPaginatedAsync(page, logsAmount)
            : await _logService.GetLogsByPerformedActionPaginatedAsync(logsActionFilter, page, logsAmount);

        int totalPages = (int)Math.Ceiling((double)totalLogsCount / logsAmount);

        var items = paginatedLogs.Select(p => new LogListItemViewModel
        {
            Id = p.Id,
            UserId = p.UserId,
            PerformedAction = p.PerformedAction,
            TimeStamp = p.TimeStamp
        });

        var paginationViewModel = new PaginationViewModel
        {
            CurrentPage = page,
            TotalPages = totalPages,
            TotalItems = totalLogsCount,
            ItemsPerPage = logsAmount,
            ControllerName = "Logs",
            ActionName = "LogsList",
        };

        var model = new LogListViewModel
        {
            Items = items.ToList(),
            Pagination = paginationViewModel
        };

        return View(model);
    }

    [HttpGet("view/{id}")]
    public async Task<IActionResult> View(long id)
    {
        Models.Log? log = await _logService.GetLogByIdAsync(id);

        if (log == null)
        {
            return NotFound();
        }

        var viewLogModel = new LogListItemViewModel
        {
            Id = log.Id,
            UserId = log.UserId,
            PerformedAction = log.PerformedAction,
            TimeStamp = log.TimeStamp
        };

        return View(viewLogModel);
    }
}
