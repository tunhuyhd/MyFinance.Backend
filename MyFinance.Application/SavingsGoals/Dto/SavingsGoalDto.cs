using MyFinance.Domain.Enums;

namespace MyFinance.Application.SavingsGoals.Dto;

public record SavingsGoalDto(
    Guid Id,
    string Name,
    decimal TargetAmount,
    decimal CurrentAmount,
    decimal RemainingAmount,
    double Percentage,
    DateTime? Deadline,
    string Icon,
    string Color,
    SavingsGoalStatus Status);

public record CreateSavingsGoalRequest(string Name, decimal TargetAmount, DateTime? Deadline, string Icon, string Color);
public record UpdateSavingsGoalRequest(string Name, decimal TargetAmount, DateTime? Deadline, string Icon, string Color);
public record ContributeRequest(decimal Amount);
