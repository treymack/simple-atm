using SimpleATM.Domain.DTOs;
using SimpleATM.Domain.Entities;
using SimpleATM.Domain.Services;
using Xunit;

namespace SimpleATM.Domain.Tests;

public class AccountsServiceTests
{
    private readonly MockAccountsRepository _mockRepository;
    private readonly AccountsService _accountsService;

    public AccountsServiceTests()
    {
        _mockRepository = new MockAccountsRepository();
        _accountsService = new AccountsService(_mockRepository);
    }

    #region GetAccountsAsync Tests

    [Fact]
    public async Task GetAccountsAsync_ShouldReturnAllAccounts_WhenSuccessful()
    {
        // Act
        var result = await _accountsService.GetAccountsAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(4, result.Value.Count());
    }

    #endregion

    #region GetAccountDetailsAsync Tests

    [Fact]
    public async Task GetAccountDetailsAsync_ShouldReturnAccountWithDetails_WhenAccountExists()
    {
        // Arrange
        int accountId = 1;

        // Act
        var result = await _accountsService.GetAccountDetailsAsync(accountId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(accountId, result.Value.Account.Id);
        Assert.Equal(1000m, result.Value.Account.Balance);
    }

    [Fact]
    public async Task GetAccountDetailsAsync_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        int invalidAccountId = 999;

        // Act
        var result = await _accountsService.GetAccountDetailsAsync(invalidAccountId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.Status == Ardalis.Result.ResultStatus.NotFound);
    }

    #endregion

    #region DepositAsync Tests

    [Fact]
    public async Task DepositAsync_ShouldSuccessfullyDepositAmount_WhenAmountIsValid()
    {
        // Arrange
        int accountId = 1;
        decimal depositAmount = 500m;
        var request = new DepositRequest(depositAmount);
        var initialAccount = (await _mockRepository.GetAccounts()).First(a => a.Id == accountId);

        // Act
        var result = await _accountsService.DepositAsync(accountId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(initialAccount.Balance + depositAmount, result.Value.Balance);
    }

    [Fact]
    public async Task DepositAsync_ShouldReturnError_WhenAmountIsZero()
    {
        // Arrange
        int accountId = 1;
        var request = new DepositRequest(0m);

        // Act
        var result = await _accountsService.DepositAsync(accountId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("greater than zero", result.Errors.First());
    }

    [Fact]
    public async Task DepositAsync_ShouldReturnError_WhenAmountIsNegative()
    {
        // Arrange
        int accountId = 1;
        var request = new DepositRequest(-100m);

        // Act
        var result = await _accountsService.DepositAsync(accountId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("greater than zero", result.Errors.First());
    }

    [Fact]
    public async Task DepositAsync_ShouldReturnError_WhenAmountExceedsMaximumLimit()
    {
        // Arrange
        int accountId = 1;
        var request = new DepositRequest(10001m);

        // Act
        var result = await _accountsService.DepositAsync(accountId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("exceeds the maximum limit", result.Errors.First());
    }

    [Fact]
    public async Task DepositAsync_ShouldAcceptDepositAtMaximumLimit()
    {
        // Arrange
        int accountId = 1;
        var request = new DepositRequest(10000m);

        // Act
        var result = await _accountsService.DepositAsync(accountId, request);

        // Assert
        Assert.True(result.IsSuccess);
    }

    #endregion

    #region WithdrawAsync Tests

    [Fact]
    public async Task WithdrawAsync_ShouldSuccessfullyWithdrawAmount_WhenAmountIsValidAndFundsAvailable()
    {
        // Arrange
        int accountId = 1;
        decimal withdrawAmount = 500m;
        var request = new WithdrawalRequest(withdrawAmount);
        var initialAccount = (await _mockRepository.GetAccounts()).First(a => a.Id == accountId);

        // Act
        var result = await _accountsService.WithdrawAsync(accountId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(initialAccount.Balance - withdrawAmount, result.Value.Balance);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldReturnError_WhenAmountIsZero()
    {
        // Arrange
        int accountId = 1;
        var request = new WithdrawalRequest(0m);

        // Act
        var result = await _accountsService.WithdrawAsync(accountId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("greater than zero", result.Errors.First());
    }

    [Fact]
    public async Task WithdrawAsync_ShouldReturnError_WhenAmountIsNegative()
    {
        // Arrange
        int accountId = 1;
        var request = new WithdrawalRequest(-100m);

        // Act
        var result = await _accountsService.WithdrawAsync(accountId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("greater than zero", result.Errors.First());
    }

    [Fact]
    public async Task WithdrawAsync_ShouldReturnError_WhenInsufficientFunds()
    {
        // Arrange
        int accountId = 3;
        decimal withdrawAmount = 1000m; // Account 3 has 500m
        var request = new WithdrawalRequest(withdrawAmount);

        // Act
        var result = await _accountsService.WithdrawAsync(accountId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Insufficient funds", result.Errors.First());
    }

    [Fact]
    public async Task WithdrawAsync_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        int invalidAccountId = 999;
        var request = new WithdrawalRequest(100m);

        // Act
        var result = await _accountsService.WithdrawAsync(invalidAccountId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.Status == Ardalis.Result.ResultStatus.NotFound);
    }

    #endregion

    #region TransferAsync Tests

    [Fact]
    public async Task TransferAsync_ShouldSuccessfullyTransferAmount_WhenBothAccountsAreValidAndFundsAvailable()
    {
        // Arrange
        var transferRequest = new TransferRequest(
            FromAccountId: 1,
            ToAccountId: 2,
            Amount: 500m,
            Description: "Test Transfer");
        var fromAccountBefore = (await _mockRepository.GetAccounts()).First(a => a.Id == 1);
        var toAccountBefore = (await _mockRepository.GetAccounts()).First(a => a.Id == 2);

        // Act
        var result = await _accountsService.TransferAsync(transferRequest);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(fromAccountBefore.Balance - transferRequest.Amount, result.Value.From.Balance);
        Assert.Equal(toAccountBefore.Balance + transferRequest.Amount, result.Value.To.Balance);
    }

    [Fact]
    public async Task TransferAsync_ShouldReturnError_WhenFromAccountHasInsufficientFunds()
    {
        // Arrange
        var transferRequest = new TransferRequest(
            FromAccountId: 3,
            ToAccountId: 2,
            Amount: 1000m, // Account 3 has 500m
            Description: "Test Transfer");

        // Act
        var result = await _accountsService.TransferAsync(transferRequest);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task TransferAsync_ShouldReturnError_WhenFromAccountDoesNotExist()
    {
        // Arrange
        var transferRequest = new TransferRequest(
            FromAccountId: 999,
            ToAccountId: 2,
            Amount: 100m,
            Description: "Test Transfer");

        // Act
        var result = await _accountsService.TransferAsync(transferRequest);

        // Assert
        Assert.False(result.IsSuccess);
    }

    #endregion
}
