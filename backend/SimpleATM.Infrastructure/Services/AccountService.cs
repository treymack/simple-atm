using Ardalis.Result;
using SimpleATM.Domain.Entities;
using SimpleATM.Domain.Services;

namespace SimpleATM.Infrastructure.Services;

public class AccountService : IAccountsService
{
    public Result<IEnumerable<Account>> GetAccounts()
    {
        if (Random.Shared.Next(1, 10) <= 2)
        {
            return Result<IEnumerable<Account>>.Error("Random failure occurred while fetching accounts.");
        }

        return new List<Account>
        {
            new Account(1, AccountType.Checking, 1500.75m),
            new Account(2, AccountType.Savings, 3200.00m),
        };
    }
}
