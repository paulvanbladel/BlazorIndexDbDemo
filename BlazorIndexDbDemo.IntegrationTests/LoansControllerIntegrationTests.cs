using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text.Json;
using Xunit;
using BlazorIndexDbDemo.Models;
using BlazorIndexDbDemo.Services;

namespace BlazorIndexDbDemo.IntegrationTests;

public class LoansControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public LoansControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetLoans_ReturnsLoanEnvelope_WithCorrectStructure()
    {
        // Act
        var response = await _client.GetAsync("/api/loans");

        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var envelope = JsonSerializer.Deserialize<LoanEnvelope>(content, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        Assert.NotNull(envelope);
        Assert.NotNull(envelope.Version);
        Assert.NotNull(envelope.Data);
        Assert.True(envelope.Data.Count() > 0);
        Assert.True(envelope.Timestamp > DateTime.MinValue);
    }

    [Fact]
    public async Task GetLoans_ReturnsExpectedNumberOfLoans()
    {
        // Act
        var response = await _client.GetAsync("/api/loans");

        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var envelope = JsonSerializer.Deserialize<LoanEnvelope>(content, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        Assert.NotNull(envelope);
        Assert.Equal(10000, envelope.Data.Count()); // Expected number based on controller
    }

    [Fact]
    public async Task GetLoans_EachLoanHasRequiredProperties()
    {
        // Act
        var response = await _client.GetAsync("/api/loans");

        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var envelope = JsonSerializer.Deserialize<LoanEnvelope>(content, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        Assert.NotNull(envelope);
        var firstLoan = envelope.Data.First();
        
        Assert.True(firstLoan.Id > 0);
        Assert.NotNull(firstLoan.Name);
        Assert.True(firstLoan.Amount > 0);
        Assert.True(firstLoan.InterestRate > 0);
    }

    [Fact]
    public async Task ValidateCache_WithValidVersion_ReturnsIsValidTrue()
    {
        // Arrange - First get the current version
        var loansResponse = await _client.GetAsync("/api/loans");
        loansResponse.EnsureSuccessStatusCode();
        
        var content = await loansResponse.Content.ReadAsStringAsync();
        var envelope = JsonSerializer.Deserialize<LoanEnvelope>(content, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        var currentVersion = envelope?.Version;
        Assert.NotNull(currentVersion);

        // Act
        var validationResponse = await _client.GetAsync($"/api/loans/validate?version={currentVersion}");

        // Assert
        validationResponse.EnsureSuccessStatusCode();
        
        var validationContent = await validationResponse.Content.ReadAsStringAsync();
        var validationResult = JsonSerializer.Deserialize<JsonElement>(validationContent);

        Assert.True(validationResult.GetProperty("isValid").GetBoolean());
        Assert.Equal(currentVersion, validationResult.GetProperty("currentVersion").GetString());
        Assert.Equal(currentVersion, validationResult.GetProperty("providedVersion").GetString());
    }

    [Fact]
    public async Task ValidateCache_WithInvalidVersion_ReturnsIsValidFalse()
    {
        // Arrange
        const string invalidVersion = "invalid-version-123";

        // Act
        var validationResponse = await _client.GetAsync($"/api/loans/validate?version={invalidVersion}");

        // Assert
        validationResponse.EnsureSuccessStatusCode();
        
        var validationContent = await validationResponse.Content.ReadAsStringAsync();
        var validationResult = JsonSerializer.Deserialize<JsonElement>(validationContent);

        Assert.False(validationResult.GetProperty("isValid").GetBoolean());
        Assert.NotEqual(invalidVersion, validationResult.GetProperty("currentVersion").GetString());
        Assert.Equal(invalidVersion, validationResult.GetProperty("providedVersion").GetString());
    }

    [Fact]
    public async Task GetLoans_HasConfigurableDelay()
    {
        // Act
        var startTime = DateTime.UtcNow;
        var response = await _client.GetAsync("/api/loans");
        var endTime = DateTime.UtcNow;

        // Assert
        response.EnsureSuccessStatusCode();
        
        var elapsed = endTime - startTime;
        Assert.True(elapsed.TotalMilliseconds >= 2000); // Should take at least 2 seconds due to delay
    }
}