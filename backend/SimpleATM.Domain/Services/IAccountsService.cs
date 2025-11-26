using Ardalis.Result;
using SimpleATM.Domain.DTOs;
using SimpleATM.Domain.Entities;

namespace SimpleATM.Domain.Services;

public interface IAccountsService
{
    Task<Result<IEnumerable<Account>>> GetAccountsAsync();
    Task<Result<Account>> DepositAsync(int accountId, DepositRequest req);
    Task<Result<Account>> WithdrawAsync(int accountId, WithdrawalRequest req);
    Task<Result<AccountWithDetails>> GetAccountDetailsAsync(int accountId);
    Task<Result<TransferResponse>> TransferAsync(TransferRequest transferRequest);
}
