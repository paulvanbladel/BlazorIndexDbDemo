using BlazorIndexDbDemo.Client.Data;
using BlazorIndexDbDemo.Client.Services;

namespace BlazorIndexDbDemo.Services;

/// <summary>
/// Server-side stub implementation of ILoanCacheService that does nothing during prerendering.
/// The real IndexedDB implementation will be used on the client side.
/// </summary>
public class ServerLoanCacheService : ILoanCacheService
{
    public Task ClearLoansAsync()
    {
        // Do nothing during server-side prerendering
        return Task.CompletedTask;
    }

    public Task StoreLoanEnvelopeAsync(LoanEnvelope envelope)
    {
        // Do nothing during server-side prerendering
        return Task.CompletedTask;
    }

    public Task<string?> GetCachedVersionAsync()
    {
        // Return null during server-side prerendering
        return Task.FromResult<string?>(null);
    }

    public Task<List<Loan>> GetAllLoansAsync()
    {
        // Return empty list during server-side prerendering
        return Task.FromResult(new List<Loan>());
    }

    public Task<object?> GetCachedMetadataAsync()
    {
        // Return null during server-side prerendering
        return Task.FromResult<object?>(null);
    }
}