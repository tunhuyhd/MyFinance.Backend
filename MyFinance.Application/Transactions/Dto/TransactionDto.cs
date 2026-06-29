using MyFinance.Domain.Enums;

namespace MyFinance.Application.Transactions.Dto;

public record TransactionDto(
    Guid Id,
    Guid AccountId,
    string AccountName,
    Guid? CategoryId,
    string? CategoryName,
    string? CategoryIcon,
    string? CategoryColor,
    decimal Amount,
    TransactionType Type,
    string Description,
    string? Note,
    DateTime TransactionDate,
    Guid? ToAccountId,
    string? ToAccountName);

public record CreateTransactionRequest(
    Guid AccountId,
    Guid? CategoryId,
    decimal Amount,
    TransactionType Type,
    string Description,
    string? Note,
    DateTime TransactionDate,
    Guid? ToAccountId);

public record UpdateTransactionRequest(
    Guid? CategoryId,
    decimal Amount,
    string Description,
    string? Note,
    DateTime TransactionDate);

public record TransactionFilterRequest(
    int Page = 1,
    int PageSize = 20,
    Guid? AccountId = null,
    Guid? CategoryId = null,
    TransactionType? Type = null,
    DateTime? From = null,
    DateTime? To = null,
    string? Search = null);

public record PagedResult<T>(List<T> Items, int Total, int Page, int PageSize);
