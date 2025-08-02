using System.ComponentModel.DataAnnotations;

namespace BlazorIndexDbDemo.Client.Data;

public class LoanEnvelope
{
    public string Version { get; set; } = default!;
    public IEnumerable<Loan> Data { get; set; } = default!;
    public DateTime Timestamp { get; set; }
}

public class CacheValidationResponse
{
    public bool IsValid { get; set; }
    public string CurrentVersion { get; set; } = default!;
    public string ProvidedVersion { get; set; } = default!;
}