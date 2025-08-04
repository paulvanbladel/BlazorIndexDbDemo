using Xunit;
using Bunit;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using BlazorIndexDbDemo.Client.Services;
using BlazorIndexDbDemo.Client.Data;

namespace BlazorIndexDbDemo.Client.Tests.Services;

public class LoanCacheServiceTests : TestContext
{
    [Fact]
    public async Task ClearLoansAsync_CallsJavaScriptFunction_Successfully()
    {
        // Arrange
        var service = new LoanCacheService(JSInterop.JSRuntime);
        JSInterop.SetupVoid("loanCacheDB.clearLoans");

        // Act
        await service.ClearLoansAsync();

        // Assert
        JSInterop.VerifyInvoke("loanCacheDB.clearLoans");
    }

    [Fact]
    public async Task StoreLoanEnvelopeAsync_CallsJavaScriptFunction_WithCorrectParameters()
    {
        // Arrange
        var service = new LoanCacheService(JSInterop.JSRuntime);
        var envelope = new LoanEnvelope
        {
            Version = "test-version",
            Data = new List<Loan>
            {
                new() { Id = 1, Name = "Test Loan", Amount = 1000, InterestRate = 5.0m }
            },
            Timestamp = DateTime.UtcNow
        };

        JSInterop.SetupVoid("loanCacheDB.storeLoanEnvelope", _ => true);

        // Act
        await service.StoreLoanEnvelopeAsync(envelope);

        // Assert
        JSInterop.VerifyInvoke("loanCacheDB.storeLoanEnvelope");
    }

    [Fact]
    public async Task GetCachedVersionAsync_ReturnsVersion_WhenJavaScriptReturnsValue()
    {
        // Arrange
        var service = new LoanCacheService(JSInterop.JSRuntime);
        const string expectedVersion = "v1.2.3";
        JSInterop.Setup<string>("loanCacheDB.getCachedVersion").SetResult(expectedVersion);

        // Act
        var result = await service.GetCachedVersionAsync();

        // Assert
        Assert.Equal(expectedVersion, result);
        JSInterop.VerifyInvoke("loanCacheDB.getCachedVersion");
    }

    [Fact]
    public async Task GetAllLoansAsync_ReturnsLoans_WhenJavaScriptReturnsData()
    {
        // Arrange
        var service = new LoanCacheService(JSInterop.JSRuntime);
        var expectedLoans = new[]
        {
            new Loan { Id = 1, Name = "Loan 1", Amount = 1000, InterestRate = 3.5m },
            new Loan { Id = 2, Name = "Loan 2", Amount = 2000, InterestRate = 4.0m }
        };

        JSInterop.Setup<Loan[]>("loanCacheDB.getAllLoans").SetResult(expectedLoans);

        // Act
        var result = await service.GetAllLoansAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Loan 1", result[0].Name);
        Assert.Equal("Loan 2", result[1].Name);
        JSInterop.VerifyInvoke("loanCacheDB.getAllLoans");
    }

    [Fact]
    public async Task GetCachedMetadataAsync_ReturnsMetadata_WhenJavaScriptReturnsValue()
    {
        // Arrange
        var service = new LoanCacheService(JSInterop.JSRuntime);
        var expectedMetadata = new { version = "v1.0", timestamp = DateTime.UtcNow };
        JSInterop.Setup<object>("loanCacheDB.getCachedMetadata").SetResult(expectedMetadata);

        // Act
        var result = await service.GetCachedMetadataAsync();

        // Assert
        Assert.Equal(expectedMetadata, result);
        JSInterop.VerifyInvoke("loanCacheDB.getCachedMetadata");
    }
}