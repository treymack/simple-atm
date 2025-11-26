namespace SimpleATM.Domain.DTOs;

public record TransferRequest(int FromAccountId, int ToAccountId, decimal Amount, string Description);
