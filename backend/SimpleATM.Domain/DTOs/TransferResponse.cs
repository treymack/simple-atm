using SimpleATM.Domain.Entities; // Probably need a Fitness test to force a new DTO to be created

namespace SimpleATM.Domain.DTOs;

public record TransferResponse(Account From, Account To);
