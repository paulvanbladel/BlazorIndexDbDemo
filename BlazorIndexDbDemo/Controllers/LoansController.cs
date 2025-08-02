using Microsoft.AspNetCore.Mvc;
using BlazorIndexDbDemo.Models;
using BlazorIndexDbDemo.Services;

namespace BlazorIndexDbDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanHashService _loanHashService;

    public LoansController(ILoanHashService loanHashService)
    {
        _loanHashService = loanHashService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLoans()
    {
        // Add configurable delay to demonstrate cache performance benefits
        await Task.Delay(2000);

        var loans = Enumerable.Range(1, 10_000).Select(i => new Loan
        {
            Id = i,
            Name = $"Loan #{i}",
            Amount = Random.Shared.Next(1000, 100000),
            InterestRate = (decimal)Random.Shared.NextDouble() * 5 + 1
        });

        var envelope = new LoanEnvelope
        {
            Version = _loanHashService.GetCurrentVersion(),
            Data = loans,
            Timestamp = DateTime.UtcNow
        };

        return Ok(envelope);
    }

    [HttpGet("validate")]
    public IActionResult ValidateCache([FromQuery] string version)
    {
        var currentVersion = _loanHashService.GetCurrentVersion();
        var isValid = version == currentVersion;
        
        return Ok(new { 
            IsValid = isValid, 
            CurrentVersion = currentVersion,
            ProvidedVersion = version
        });
    }
}