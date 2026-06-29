using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Budgets.Dto;
using MyFinance.Application.Categories.Dto;
using MyFinance.Application.Common.Interfaces;

namespace MyFinance.Application.Budgets.Queries;

public record GetBudgetsQuery(int Month, int Year) : IRequest<List<BudgetDto>>;

public class GetBudgetsQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetBudgetsQuery, List<BudgetDto>>
{
    public async Task<List<BudgetDto>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
    {
        var budgets = await context.Budgets
            .Include(b => b.Category)
            .Where(b => b.UserId == currentUser.UserId && b.Month == request.Month && b.Year == request.Year)
            .Select(b => new
            {
                Budget = b,
                SpentAmount = context.Transactions
                    .Where(t => t.AccountId != null && t.CategoryId == b.CategoryId 
                           && t.TransactionDate.Month == request.Month 
                           && t.TransactionDate.Year == request.Year 
                           && t.Type == Domain.Enums.TransactionType.Expense)
                    .Sum(t => (decimal?)t.Amount) ?? 0m
            })
            .ToListAsync(cancellationToken);

        return budgets.Select(x => new BudgetDto(
            x.Budget.Id,
            new CategoryDto(x.Budget.Category.Id, x.Budget.Category.Name, x.Budget.Category.Type, x.Budget.Category.Icon, x.Budget.Category.Color, x.Budget.Category.IsSystem),
            x.Budget.LimitAmount,
            x.SpentAmount,
            x.Budget.LimitAmount - x.SpentAmount,
            x.Budget.Month,
            x.Budget.Year,
            x.Budget.LimitAmount > 0 ? (double)x.SpentAmount / (double)x.Budget.LimitAmount * 100 : 0
        )).ToList();
    }
}
