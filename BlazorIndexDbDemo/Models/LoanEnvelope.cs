namespace BlazorIndexDbDemo.Models;

public class LoanEnvelope
{
    public string Version { get; set; } = default!;
    public IEnumerable<Loan> Data { get; set; } = default!;
    public DateTime Timestamp { get; set; }
}

public class Loan
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
}