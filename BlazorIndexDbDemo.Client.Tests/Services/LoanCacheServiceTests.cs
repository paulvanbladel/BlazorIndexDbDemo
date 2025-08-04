using Xunit;
using Microsoft.JSInterop;
using BlazorIndexDbDemo.Client.Services;
using BlazorIndexDbDemo.Client.Data;
using Moq;
using Microsoft.JSInterop.Infrastructure;

namespace BlazorIndexDbDemo.Client.Tests.Services;

public class LoanCacheServiceTests
{
    [Fact]
    public async Task ClearLoansAsync_CallsJavaScriptFunction_Successfully()
    {
        // Arrange
        var mockJSRuntime = new Mock<IJSRuntime>();
        mockJSRuntime.Setup(x => x.InvokeAsync<IJSVoidResult>("loanCacheDB.clearLoans", It.IsAny<object[]>()))
                    .Returns(ValueTask.FromResult(Mock.Of<IJSVoidResult>()));

        var service = new LoanCacheService(mockJSRuntime.Object);

        // Act
        await service.ClearLoansAsync();

        // Assert
        mockJSRuntime.Verify(x => x.InvokeAsync<IJSVoidResult>("loanCacheDB.clearLoans", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task StoreLoanEnvelopeAsync_CallsJavaScriptFunction_WithCorrectParameters()
    {
        // Arrange
        var mockJSRuntime = new Mock<IJSRuntime>();
        mockJSRuntime.Setup(x => x.InvokeAsync<IJSVoidResult>("loanCacheDB.storeLoanEnvelope", It.IsAny<object[]>()))
                    .Returns(ValueTask.FromResult(Mock.Of<IJSVoidResult>()));

        var service = new LoanCacheService(mockJSRuntime.Object);
        var envelope = new LoanEnvelope
        {
            Version = "test-version",
            Data = new List<Loan>
            {
                new() { Id = 1, Name = "Test Loan", Amount = 1000, InterestRate = 5.0m }
            },
            Timestamp = DateTime.UtcNow
        };

        // Act
        await service.StoreLoanEnvelopeAsync(envelope);

        // Assert
        mockJSRuntime.Verify(x => x.InvokeAsync<IJSVoidResult>("loanCacheDB.storeLoanEnvelope", It.Is<object[]>(args => args.Length == 1 && args[0] == envelope)), Times.Once);
    }

    [Fact]
    public async Task GetCachedVersionAsync_ReturnsVersion_WhenJavaScriptReturnsValue()
    {
        // Arrange
        var mockJSRuntime = new Mock<IJSRuntime>();
        const string expectedVersion = "v1.2.3";
        mockJSRuntime.Setup(x => x.InvokeAsync<string>("loanCacheDB.getCachedVersion", It.IsAny<object[]>()))
                    .Returns(ValueTask.FromResult(expectedVersion));

        var service = new LoanCacheService(mockJSRuntime.Object);

        // Act
        var result = await service.GetCachedVersionAsync();

        // Assert
        Assert.Equal(expectedVersion, result);
        mockJSRuntime.Verify(x => x.InvokeAsync<string>("loanCacheDB.getCachedVersion", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task GetCachedVersionAsync_ReturnsNull_WhenJavaScriptThrowsException()
    {
        // Arrange
        var mockJSRuntime = new Mock<IJSRuntime>();
        mockJSRuntime.Setup(x => x.InvokeAsync<string>("loanCacheDB.getCachedVersion", It.IsAny<object[]>()))
                    .Throws(new JSException("JavaScript error"));

        var service = new LoanCacheService(mockJSRuntime.Object);

        // Act
        var result = await service.GetCachedVersionAsync();

        // Assert
        Assert.Null(result);
        mockJSRuntime.Verify(x => x.InvokeAsync<string>("loanCacheDB.getCachedVersion", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task GetAllLoansAsync_ReturnsLoans_WhenJavaScriptReturnsData()
    {
        // Arrange
        var mockJSRuntime = new Mock<IJSRuntime>();
        var expectedLoans = new[]
        {
            new Loan { Id = 1, Name = "Loan 1", Amount = 1000, InterestRate = 3.5m },
            new Loan { Id = 2, Name = "Loan 2", Amount = 2000, InterestRate = 4.0m }
        };
        mockJSRuntime.Setup(x => x.InvokeAsync<Loan[]>("loanCacheDB.getAllLoans", It.IsAny<object[]>()))
                    .Returns(ValueTask.FromResult(expectedLoans));

        var service = new LoanCacheService(mockJSRuntime.Object);

        // Act
        var result = await service.GetAllLoansAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Loan 1", result[0].Name);
        Assert.Equal("Loan 2", result[1].Name);
        mockJSRuntime.Verify(x => x.InvokeAsync<Loan[]>("loanCacheDB.getAllLoans", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task GetAllLoansAsync_ReturnsEmptyList_WhenJavaScriptThrowsException()
    {
        // Arrange
        var mockJSRuntime = new Mock<IJSRuntime>();
        mockJSRuntime.Setup(x => x.InvokeAsync<Loan[]>("loanCacheDB.getAllLoans", It.IsAny<object[]>()))
                    .Throws(new JSException("JavaScript error"));

        var service = new LoanCacheService(mockJSRuntime.Object);

        // Act
        var result = await service.GetAllLoansAsync();

        // Assert
        Assert.Empty(result);
        mockJSRuntime.Verify(x => x.InvokeAsync<Loan[]>("loanCacheDB.getAllLoans", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task GetAllLoansAsync_ReturnsEmptyList_WhenJavaScriptReturnsNull()
    {
        // Arrange
        var mockJSRuntime = new Mock<IJSRuntime>();
        mockJSRuntime.Setup(x => x.InvokeAsync<Loan[]>("loanCacheDB.getAllLoans", It.IsAny<object[]>()))
                    .Returns(ValueTask.FromResult((Loan[])null!));

        var service = new LoanCacheService(mockJSRuntime.Object);

        // Act
        var result = await service.GetAllLoansAsync();

        // Assert
        Assert.Empty(result);
        mockJSRuntime.Verify(x => x.InvokeAsync<Loan[]>("loanCacheDB.getAllLoans", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task GetCachedMetadataAsync_ReturnsMetadata_WhenJavaScriptReturnsValue()
    {
        // Arrange
        var mockJSRuntime = new Mock<IJSRuntime>();
        var expectedMetadata = new { version = "v1.0", timestamp = DateTime.UtcNow };
        mockJSRuntime.Setup(x => x.InvokeAsync<object>("loanCacheDB.getCachedMetadata", It.IsAny<object[]>()))
                    .Returns(ValueTask.FromResult((object)expectedMetadata));

        var service = new LoanCacheService(mockJSRuntime.Object);

        // Act
        var result = await service.GetCachedMetadataAsync();

        // Assert
        Assert.Equal(expectedMetadata, result);
        mockJSRuntime.Verify(x => x.InvokeAsync<object>("loanCacheDB.getCachedMetadata", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task GetCachedMetadataAsync_ReturnsNull_WhenJavaScriptThrowsException()
    {
        // Arrange
        var mockJSRuntime = new Mock<IJSRuntime>();
        mockJSRuntime.Setup(x => x.InvokeAsync<object>("loanCacheDB.getCachedMetadata", It.IsAny<object[]>()))
                    .Throws(new JSException("JavaScript error"));

        var service = new LoanCacheService(mockJSRuntime.Object);

        // Act
        var result = await service.GetCachedMetadataAsync();

        // Assert
        Assert.Null(result);
        mockJSRuntime.Verify(x => x.InvokeAsync<object>("loanCacheDB.getCachedMetadata", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task ClearLoansAsync_ThrowsException_WhenJavaScriptThrowsException()
    {
        // Arrange
        var mockJSRuntime = new Mock<IJSRuntime>();
        var expectedException = new JSException("JavaScript error");
        mockJSRuntime.Setup(x => x.InvokeAsync<IJSVoidResult>("loanCacheDB.clearLoans", It.IsAny<object[]>()))
                    .Throws(expectedException);

        var service = new LoanCacheService(mockJSRuntime.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<JSException>(() => service.ClearLoansAsync());
        Assert.Equal("JavaScript error", exception.Message);
        mockJSRuntime.Verify(x => x.InvokeAsync<IJSVoidResult>("loanCacheDB.clearLoans", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task StoreLoanEnvelopeAsync_ThrowsException_WhenJavaScriptThrowsException()
    {
        // Arrange
        var mockJSRuntime = new Mock<IJSRuntime>();
        var expectedException = new JSException("JavaScript error");
        mockJSRuntime.Setup(x => x.InvokeAsync<IJSVoidResult>("loanCacheDB.storeLoanEnvelope", It.IsAny<object[]>()))
                    .Throws(expectedException);

        var service = new LoanCacheService(mockJSRuntime.Object);
        var envelope = new LoanEnvelope
        {
            Version = "test-version",
            Data = new List<Loan>
            {
                new() { Id = 1, Name = "Test Loan", Amount = 1000, InterestRate = 5.0m }
            },
            Timestamp = DateTime.UtcNow
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<JSException>(() => service.StoreLoanEnvelopeAsync(envelope));
        Assert.Equal("JavaScript error", exception.Message);
        mockJSRuntime.Verify(x => x.InvokeAsync<IJSVoidResult>("loanCacheDB.storeLoanEnvelope", It.Is<object[]>(args => args.Length == 1 && args[0] == envelope)), Times.Once);
    }
}