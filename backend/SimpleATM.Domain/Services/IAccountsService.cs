using Ardalis.Result;
using SimpleATM.Domain.Entities;

namespace SimpleATM.Domain.Services;

public interface IAccountsService
{
    Result<IEnumerable<Account>> GetAccounts();
}
