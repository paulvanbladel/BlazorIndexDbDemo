using Microsoft.AspNetCore.Mvc;
using BlazorIndexDbDemo.Services;

namespace BlazorIndexDbDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoanConfigController : ControllerBase
{
    private readonly ILoanHashService _loanHashService;

    public LoanConfigController(ILoanHashService loanHashService)
    {
        _loanHashService = loanHashService;
    }

    [HttpPost("invalidate")]
    public IActionResult InvalidateCache()
    {
        var oldVersion = _loanHashService.GetCurrentVersion();
        _loanHashService.InvalidateCache();
        var newVersion = _loanHashService.GetCurrentVersion();

        return Ok(new 
        { 
            Message = "Cache invalidated successfully", 
            OldVersion = oldVersion,
            NewVersion = newVersion,
            Timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("version")]
    public IActionResult GetCurrentVersion()
    {
        return Ok(new 
        { 
            Version = _loanHashService.GetCurrentVersion(),
            Timestamp = DateTime.UtcNow
        });
    }

    [HttpPost("reset")]
    public IActionResult ResetToFresh()
    {
        var oldVersion = _loanHashService.GetCurrentVersion();
        _loanHashService.ResetToFresh();
        var newVersion = _loanHashService.GetCurrentVersion();

        return Ok(new 
        { 
            Message = "Cache reset to fresh state", 
            OldVersion = oldVersion,
            NewVersion = newVersion,
            Timestamp = DateTime.UtcNow
        });
    }
}