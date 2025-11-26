using Ardalis.Result;
using SimpleATM.Domain.DTOs;
using SimpleATM.Domain.Entities;
using SimpleATM.Domain.Services;

namespace SimpleATM.Infrastructure.Services;

public class AccountsService(IAccountsRepository accountsRepository) : IAccountsService
{
    private const decimal MAX_DEPOSIT_AMOUNT = 10000m;

    public async Task<Result<IEnumerable<Account>>> GetAccountsAsync()
    {
        if (Random.Shared.Next(1, 10) <= 2)
        {
            return Result<IEnumerable<Account>>.Error("Random failure occurred while fetching accounts.");
        }

        return Result.Success(await accountsRepository.GetAccounts());
    }

    public async Task<Result<AccountWithDetails>> GetAccountDetailsAsync(int accountId)
    {
        // Definitely not optimal, but there are 4 accounts in SimpleATM
        var accounts = await accountsRepository.GetAccounts();
        var account = accounts.FirstOrDefault(x => x.Id == accountId);

        if (account is null)
        {
            return Result<AccountWithDetails>.NotFound();
        }

        var transactions = await accountsRepository.GetAccountTransactions(accountId);
        var accountWithDetails = new AccountWithDetails(account, transactions);

        return accountWithDetails;
    }

    public async Task<Result<Account>> DepositAsync(int accountId, DepositRequest req)
    {
        if (req.Amount <= 0)
        {
            return Result<Account>.Error("Deposit amount must be greater than zero.");
        }

        if (req.Amount > MAX_DEPOSIT_AMOUNT)
        {
            return Result<Account>.Error($"Deposit amount exceeds the maximum limit of {MAX_DEPOSIT_AMOUNT}.");

        }

        var account = await accountsRepository.DepositAsync(accountId, req.Amount, "Web Deposit");
        return Result.Success(account);
    }

    public async Task<Result<Account>> WithdrawAsync(int accountId, WithdrawalRequest req)
    {
        if (req.Amount <= 0)
        {
            return Result<Account>.Error("Withdrawal amount must be greater than zero.");
        }

        // Inefficient but simple way to get the balance
        // There's also a race condition here, but this is just a demo app
        var account = await GetAccountDetailsAsync(accountId);
        if (account.IsNotFound())
        {
            return Result<Account>.NotFound("Failed to retrieve account details.");
        }

        var balance = account.Value.Account.Balance;
        if (balance < req.Amount)
        {
            return Result<Account>.Error("Insufficient funds for this withdrawal.");
        }

        return await accountsRepository.WithdrawAsync(accountId, req.Amount, "Web Withdrawal");
    }
}