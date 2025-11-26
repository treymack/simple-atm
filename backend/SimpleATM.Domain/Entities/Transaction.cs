namespace SimpleATM.Domain.Entities;

public record Transaction(
    int Id,
    int FromAccountId,
    int ToAccountId,
    TransactionType type,
    decimal Amount,
    DateTime Timestamp,
    string Description
    )
{ }
