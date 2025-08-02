using System.ComponentModel.DataAnnotations;

namespace BlazorIndexDbDemo.Client.Data;

public class Loan
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
}