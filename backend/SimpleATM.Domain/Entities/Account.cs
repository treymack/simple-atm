namespace SimpleATM.Domain.Entities;

public record Account(
    int Id,
    AccountType type,
    decimal Balance)
{ }
