using MyFinance.Application.Categories.Dto;

namespace MyFinance.Application.Budgets.Dto;

public record BudgetDto(
    Guid Id,
    CategoryDto Category,
    decimal LimitAmount,
    decimal SpentAmount,
    decimal RemainingAmount,
    int Month,
    int Year,
    double Percentage);

public record CreateBudgetRequest(Guid CategoryId, decimal LimitAmount, int Month, int Year);
public record UpdateBudgetRequest(decimal LimitAmount);
