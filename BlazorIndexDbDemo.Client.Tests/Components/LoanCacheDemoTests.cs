using Xunit;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using BlazorIndexDbDemo.Client.Services;
using BlazorIndexDbDemo.Client.Data;
using BlazorIndexDbDemo.Client.Pages;

namespace BlazorIndexDbDemo.Client.Tests.Components;

public class LoanCacheDemoTests : TestContext
{
    private readonly Mock<ILoanCacheService> _mockCacheService;

    public LoanCacheDemoTests()
    {
        _mockCacheService = new Mock<ILoanCacheService>();
        Services.AddSingleton(_mockCacheService.Object);
        Services.AddSingleton<HttpClient>();
    }

    [Fact]
    public void LoanCacheDemo_RendersCorrectly_WithInitialState()
    {
        // Act
        var component = RenderComponent<LoanCacheDemo>();

        // Assert
        Assert.Contains("Loan Cache Demo with Smart Cache Invalidation", component.Markup);
        Assert.Contains("Load Fresh from API", component.Markup);
        Assert.Contains("Smart Load (Cache + Validation)", component.Markup);
        Assert.Contains("Force Load from Cache", component.Markup);
        Assert.Contains("Clear Cache", component.Markup);
    }

    [Fact]
    public void LoanCacheDemo_InitiallyShowsAllButtons()
    {
        // Act
        var component = RenderComponent<LoanCacheDemo>();

        // Assert
        var buttons = component.FindAll("button");
        Assert.True(buttons.Count >= 4); // At least 4 main buttons
        
        // Check for the main action buttons
        Assert.Contains(buttons, b => b.TextContent.Contains("Load Fresh from API"));
        Assert.Contains(buttons, b => b.TextContent.Contains("Smart Load"));
        Assert.Contains(buttons, b => b.TextContent.Contains("Force Load from Cache"));
        Assert.Contains(buttons, b => b.TextContent.Contains("Clear Cache"));
    }

    [Fact]
    public void LoanCacheDemo_AllButtonsEnabledInitially()
    {
        // Act
        var component = RenderComponent<LoanCacheDemo>();

        // Assert
        var loadFromCacheButton = component.Find("button:contains('Force Load from Cache')");
        var clearCacheButton = component.Find("button:contains('Clear Cache')");
        
        Assert.False(loadFromCacheButton.HasAttribute("disabled"));
        Assert.False(clearCacheButton.HasAttribute("disabled"));
    }

    [Fact]
    public void LoanCacheDemo_HasCorrectStructure()
    {
        // Act
        var component = RenderComponent<LoanCacheDemo>();

        // Assert
        // Check for demo controls section
        Assert.Contains("Demo Controls", component.Markup);
        Assert.Contains("Simulate Data Change", component.Markup);
        Assert.Contains("Check Server Version", component.Markup);
    }
}