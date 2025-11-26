namespace SimpleATM.Domain.Entities;

public record AccountWithDetails(
    Account Account,
    IEnumerable<Transaction> Transactions)
{ };
