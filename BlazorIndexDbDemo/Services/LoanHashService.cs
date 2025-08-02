namespace BlazorIndexDbDemo.Services;

public interface ILoanHashService
{
    string GetCurrentVersion();
    void InvalidateCache();
    void ResetToFresh();
}

public class LoanHashService : ILoanHashService
{
    private string _currentVersion;

    public LoanHashService()
    {
        _currentVersion = Guid.NewGuid().ToString();
    }

    public string GetCurrentVersion()
    {
        return _currentVersion;
    }

    public void InvalidateCache()
    {
        _currentVersion = Guid.NewGuid().ToString();
    }

    public void ResetToFresh()
    {
        _currentVersion = Guid.NewGuid().ToString();
    }
}