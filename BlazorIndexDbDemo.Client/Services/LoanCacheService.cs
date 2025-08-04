using Microsoft.JSInterop;
using BlazorIndexDbDemo.Client.Data;

namespace BlazorIndexDbDemo.Client.Services;

public class LoanCacheService : ILoanCacheService
{
    private readonly IJSRuntime _jsRuntime;

    public LoanCacheService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task ClearLoansAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("loanCacheDB.clearLoans");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing cache: {ex.Message}");
            throw;
        }
    }

    public async Task StoreLoanEnvelopeAsync(LoanEnvelope envelope)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("loanCacheDB.storeLoanEnvelope", envelope);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error storing loan envelope: {ex.Message}");
            throw;
        }
    }

    public async Task<string?> GetCachedVersionAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("loanCacheDB.getCachedVersion");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting cached version: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Loan>> GetAllLoansAsync()
    {
        try
        {
            var loans = await _jsRuntime.InvokeAsync<Loan[]>("loanCacheDB.getAllLoans");
            Console.WriteLine($"Retrieved {loans?.Length ?? 0} loans from cache");
            return loans?.ToList() ?? new List<Loan>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting cached loans: {ex.Message}");
            return new List<Loan>();
        }
    }

    public async Task<object?> GetCachedMetadataAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<object>("loanCacheDB.getCachedMetadata");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting cached metadata: {ex.Message}");
            return null;
        }
    }
}