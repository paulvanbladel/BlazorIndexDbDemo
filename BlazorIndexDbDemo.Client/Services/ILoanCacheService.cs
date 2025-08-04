using BlazorIndexDbDemo.Client.Data;

namespace BlazorIndexDbDemo.Client.Services;

public interface ILoanCacheService
{
    Task ClearLoansAsync();
    Task StoreLoanEnvelopeAsync(LoanEnvelope envelope);
    Task<string?> GetCachedVersionAsync();
    Task<List<Loan>> GetAllLoansAsync();
    Task<object?> GetCachedMetadataAsync();
}