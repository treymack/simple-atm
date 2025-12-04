using SimpleATM.Domain.Entities;
using SimpleATM.Domain.Services;

namespace SimpleATM.Domain.Tests;

/// <summary>
/// Mock repository implementation for testing.
/// </summary>
public class MockAccountsRepository : IAccountsRepository
{
    private readonly List<Account> _accounts = [];
    private readonly List<Transaction> _transactions = [];
    private IDisposable? _currentTransaction;
    private int _transactionIdCounter = 1;

    public MockAccountsRepository()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        _accounts.Clear();
        _transactions.Clear();
        _transactionIdCounter = 1;

        _accounts.AddRange(new[]
        {
            new Account(Id: 1, type: AccountType.Checking, Balance: 1000m),
            new Account(Id: 2, type: AccountType.Savings, Balance: 5000m),
            new Account(Id: 3, type: AccountType.Checking, Balance: 500m),
            new Account(Id: 4, type: AccountType.Savings, Balance: 10000m)
        });
    }

    public Task WipeAndSeedDatabaseAsync()
    {
        InitializeMockData();
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Account>> GetAccounts()
    {
        return Task.FromResult(_accounts.AsEnumerable());
    }

    public Task<Account> DepositAsync(int accountId, decimal amount, string description)
    {
        var accountIndex = _accounts.FindIndex(a => a.Id == accountId);
        if (accountIndex == -1)
            throw new InvalidOperationException($"Account {accountId} not found.");

        var account = _accounts[accountIndex];
        var updatedAccount = account with { Balance = account.Balance + amount };
        _accounts[accountIndex] = updatedAccount;

        _transactions.Add(new Transaction(
            Id: _transactionIdCounter++,
            FromAccountId: 0,
            ToAccountId: accountId,
            type: TransactionType.Deposit,
            Amount: amount,
            Timestamp: DateTime.UtcNow,
            Description: description));

        return Task.FromResult(updatedAccount);
    }

    public Task<IEnumerable<Transaction>> GetAccountTransactions(int accountId)
    {
        var transactions = _transactions.Where(t => t.ToAccountId == accountId || t.FromAccountId == accountId);
        return Task.FromResult(transactions);
    }

    public Task<Account> WithdrawAsync(int accountId, decimal amount, string description)
    {
        var accountIndex = _accounts.FindIndex(a => a.Id == accountId);
        if (accountIndex == -1)
            throw new InvalidOperationException($"Account {accountId} not found.");

        var account = _accounts[accountIndex];
        var updatedAccount = account with { Balance = account.Balance - amount };
        _accounts[accountIndex] = updatedAccount;

        _transactions.Add(new Transaction(
            Id: _transactionIdCounter++,
            FromAccountId: accountId,
            ToAccountId: 0,
            type: TransactionType.Withdrawal,
            Amount: amount,
            Timestamp: DateTime.UtcNow,
            Description: description));

        return Task.FromResult(updatedAccount);
    }

    public Task<IDisposable> BeginTransactionAsync()
    {
        _currentTransaction = new MockTransaction();
        return Task.FromResult(_currentTransaction);
    }

    public Task CommitTransactionAsync()
    {
        _currentTransaction?.Dispose();
        _currentTransaction = null;
        return Task.CompletedTask;
    }

    private class MockTransaction : IDisposable
    {
        public void Dispose() { }
    }
}
