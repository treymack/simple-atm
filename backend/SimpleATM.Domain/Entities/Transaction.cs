namespace SimpleATM.Domain.Entities;

public record Transaction(
    int Id,
    Account FromAccount,
    Account ToAccount,
    TransactionType type,
    decimal Amount,
    DateTimeOffset Timestamp,
    string? Description = null
    )
{ }
