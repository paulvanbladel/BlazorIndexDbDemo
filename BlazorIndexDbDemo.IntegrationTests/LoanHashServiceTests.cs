using Xunit;
using BlazorIndexDbDemo.Services;

namespace BlazorIndexDbDemo.IntegrationTests;

public class LoanHashServiceTests
{
    [Fact]
    public void GetCurrentVersion_ReturnsNonEmptyString()
    {
        // Arrange
        var service = new LoanHashService();

        // Act
        var version = service.GetCurrentVersion();

        // Assert
        Assert.NotNull(version);
        Assert.NotEmpty(version);
    }

    [Fact]
    public void GetCurrentVersion_ReturnsSameValueOnMultipleCalls()
    {
        // Arrange
        var service = new LoanHashService();

        // Act
        var version1 = service.GetCurrentVersion();
        var version2 = service.GetCurrentVersion();

        // Assert
        Assert.Equal(version1, version2);
    }

    [Fact]
    public void InvalidateCache_ChangesVersion()
    {
        // Arrange
        var service = new LoanHashService();
        var originalVersion = service.GetCurrentVersion();

        // Act
        service.InvalidateCache();
        var newVersion = service.GetCurrentVersion();

        // Assert
        Assert.NotEqual(originalVersion, newVersion);
    }

    [Fact]
    public void ResetToFresh_ChangesVersion()
    {
        // Arrange
        var service = new LoanHashService();
        var originalVersion = service.GetCurrentVersion();

        // Act
        service.ResetToFresh();
        var newVersion = service.GetCurrentVersion();

        // Assert
        Assert.NotEqual(originalVersion, newVersion);
    }

    [Fact]
    public void MultipleInvalidations_ProduceDifferentVersions()
    {
        // Arrange
        var service = new LoanHashService();
        var originalVersion = service.GetCurrentVersion();

        // Act
        service.InvalidateCache();
        var version1 = service.GetCurrentVersion();
        
        service.InvalidateCache();
        var version2 = service.GetCurrentVersion();

        // Assert
        Assert.NotEqual(originalVersion, version1);
        Assert.NotEqual(version1, version2);
        Assert.NotEqual(originalVersion, version2);
    }
}