using TG.Blazor.IndexedDB;
using BlazorIndexDbDemo.Client.Data;

namespace BlazorIndexDbDemo.Client.Services;

public class LoanCacheService
{
    private readonly IndexedDBManager _indexedDBManager;
    private const string LoansStoreName = "Loans";
    private const string MetadataStoreName = "Metadata";

    public LoanCacheService(IndexedDBManager indexedDBManager)
    {
        _indexedDBManager = indexedDBManager;
    }

    public async Task ClearLoansAsync()
    {
        await _indexedDBManager.ClearStore(LoansStoreName);
        await _indexedDBManager.ClearStore(MetadataStoreName);
    }

    public async Task StoreLoanEnvelopeAsync(LoanEnvelope envelope)
    {
        // Clear existing data
        await _indexedDBManager.ClearStore(LoansStoreName);
        await _indexedDBManager.ClearStore(MetadataStoreName);

        // Store metadata
        var metadata = new { key = "version", value = envelope.Version, timestamp = envelope.Timestamp };
        var metadataRecord = new StoreRecord<object>
        {
            Storename = MetadataStoreName,
            Data = metadata
        };
        await _indexedDBManager.AddRecord(metadataRecord);

        // Store loans
        foreach (var loan in envelope.Data)
        {
            var loanRecord = new StoreRecord<Loan>
            {
                Storename = LoansStoreName,
                Data = loan
            };
            await _indexedDBManager.AddRecord(loanRecord);
        }
    }

    public async Task<string?> GetCachedVersionAsync()
    {
        var metadata = await _indexedDBManager.GetRecordById<string, object>(MetadataStoreName, "version");
        
        if (metadata != null)
        {
            var metadataType = metadata.GetType();
            var valueProperty = metadataType.GetProperty("value");
            return valueProperty?.GetValue(metadata)?.ToString();
        }
        
        return null;
    }

    public async Task<List<Loan>> GetAllLoansAsync()
    {
        var loans = await _indexedDBManager.GetRecords<Loan>(LoansStoreName);
        return loans?.ToList() ?? new List<Loan>();
    }

    public async Task<object?> GetCachedMetadataAsync()
    {
        return await _indexedDBManager.GetRecordById<string, object>(MetadataStoreName, "version");
    }
}