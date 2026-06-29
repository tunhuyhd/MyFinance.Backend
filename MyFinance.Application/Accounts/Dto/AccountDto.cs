using MyFinance.Domain.Enums;

namespace MyFinance.Application.Accounts.Dto;

public record AccountDto(
    Guid Id,
    string Name,
    AccountType AccountType,
    decimal Balance,
    string Currency,
    string Color,
    string Icon,
    bool IsDefault);

public record CreateAccountRequest(
    string Name,
    AccountType AccountType,
    decimal InitialBalance,
    string Currency,
    string Color,
    string Icon,
    bool IsDefault);

public record UpdateAccountRequest(
    string Name,
    string Color,
    string Icon,
    bool IsDefault);
