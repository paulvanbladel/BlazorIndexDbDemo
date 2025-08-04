using Xunit;
using BlazorIndexDbDemo.Client.Data;

namespace BlazorIndexDbDemo.Client.Tests.Data;

public class LoanTests
{
    [Fact]
    public void Loan_CanBeCreated_WithValidData()
    {
        // Act
        var loan = new Loan
        {
            Id = 1,
            Name = "Test Loan",
            Amount = 10000.00m,
            InterestRate = 5.25m
        };

        // Assert
        Assert.Equal(1, loan.Id);
        Assert.Equal("Test Loan", loan.Name);
        Assert.Equal(10000.00m, loan.Amount);
        Assert.Equal(5.25m, loan.InterestRate);
    }

    [Fact]
    public void Loan_HasDefaultValues()
    {
        // Act
        var loan = new Loan();

        // Assert
        Assert.Equal(0, loan.Id);
        Assert.Null(loan.Name);
        Assert.Equal(default, loan.Amount);
        Assert.Equal(default, loan.InterestRate);
    }

    [Theory]
    [InlineData(1000.50, 3.25)]
    [InlineData(50000.99, 7.5)]
    [InlineData(0.01, 0.1)]
    public void Loan_CanStoreDecimalValues_Correctly(decimal amount, decimal interestRate)
    {
        // Arrange & Act
        var loan = new Loan
        {
            Amount = amount,
            InterestRate = interestRate
        };

        // Assert
        Assert.Equal(amount, loan.Amount);
        Assert.Equal(interestRate, loan.InterestRate);
    }
}

public class LoanEnvelopeTests
{
    [Fact]
    public void LoanEnvelope_CanBeCreated_WithValidData()
    {
        // Arrange
        var loans = new List<Loan>
        {
            new() { Id = 1, Name = "Loan 1", Amount = 1000, InterestRate = 3.5m },
            new() { Id = 2, Name = "Loan 2", Amount = 2000, InterestRate = 4.0m }
        };
        var timestamp = DateTime.UtcNow;

        // Act
        var envelope = new LoanEnvelope
        {
            Version = "v1.2.3",
            Data = loans,
            Timestamp = timestamp
        };

        // Assert
        Assert.Equal("v1.2.3", envelope.Version);
        Assert.Equal(loans, envelope.Data);
        Assert.Equal(timestamp, envelope.Timestamp);
    }

    [Fact]
    public void LoanEnvelope_HasDefaultValues()
    {
        // Act
        var envelope = new LoanEnvelope();

        // Assert
        Assert.Null(envelope.Version);
        Assert.Null(envelope.Data);
        Assert.Equal(default, envelope.Timestamp);
    }

    [Fact]
    public void LoanEnvelope_CanStoreEmptyLoanCollection()
    {
        // Act
        var envelope = new LoanEnvelope
        {
            Version = "v1.0",
            Data = Enumerable.Empty<Loan>(),
            Timestamp = DateTime.UtcNow
        };

        // Assert
        Assert.Empty(envelope.Data);
        Assert.Equal("v1.0", envelope.Version);
    }

    [Fact]
    public void LoanEnvelope_CanStoreLargeNumberOfLoans()
    {
        // Arrange
        const int loanCount = 10000;
        var loans = Enumerable.Range(1, loanCount)
            .Select(i => new Loan 
            { 
                Id = i, 
                Name = $"Loan {i}", 
                Amount = i * 100, 
                InterestRate = 3.5m 
            });

        // Act
        var envelope = new LoanEnvelope
        {
            Version = "v2.0",
            Data = loans,
            Timestamp = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(loanCount, envelope.Data.Count());
        Assert.Equal("v2.0", envelope.Version);
    }
}

public class CacheValidationResponseTests
{
    [Fact]
    public void CacheValidationResponse_CanBeCreated_WithValidData()
    {
        // Act
        var response = new CacheValidationResponse
        {
            IsValid = true,
            CurrentVersion = "v1.2.3",
            ProvidedVersion = "v1.2.3"
        };

        // Assert
        Assert.True(response.IsValid);
        Assert.Equal("v1.2.3", response.CurrentVersion);
        Assert.Equal("v1.2.3", response.ProvidedVersion);
    }

    [Fact]
    public void CacheValidationResponse_CanIndicateInvalidCache()
    {
        // Act
        var response = new CacheValidationResponse
        {
            IsValid = false,
            CurrentVersion = "v1.2.4",
            ProvidedVersion = "v1.2.3"
        };

        // Assert
        Assert.False(response.IsValid);
        Assert.Equal("v1.2.4", response.CurrentVersion);
        Assert.Equal("v1.2.3", response.ProvidedVersion);
    }

    [Fact]
    public void CacheValidationResponse_HasDefaultValues()
    {
        // Act
        var response = new CacheValidationResponse();

        // Assert
        Assert.False(response.IsValid);
        Assert.Null(response.CurrentVersion);
        Assert.Null(response.ProvidedVersion);
    }
}