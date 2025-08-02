using Microsoft.AspNetCore.Mvc;

namespace BlazorIndexDbDemo.Controllers;

public class Loan
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    [HttpGet]
    public IActionResult GetLoans()
    {
        var loans = Enumerable.Range(1, 10_000).Select(i => new Loan
        {
            Id = i,
            Name = $"Loan #{i}",
            Amount = Random.Shared.Next(1000, 100000),
            InterestRate = (decimal)Random.Shared.NextDouble() * 5 + 1
        });

        return Ok(loans);
    }
}