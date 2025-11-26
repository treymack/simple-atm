using SimpleATM.Domain.Entities;

namespace SimpleATM.Domain.Services;

public interface IAccountsRepository
{
    Task WipeAndSeedDatabaseAsync();
    Task<IEnumerable<Account>> GetAccounts();
    Task<Account> DepositAsync(int accountId, decimal amount, string description);
    Task<IEnumerable<Transaction>> GetAccountTransactions(int accountId);
    Task<Account> WithdrawAsync(int accountId, decimal amount, string description);
    Task<IDisposable> BeginTransactionAsync();
    Task CommitTransactionAsync();
}