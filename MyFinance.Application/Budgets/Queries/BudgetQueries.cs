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
        return await context.Budgets
            .Include(b => b.Category)
            .Where(b => b.UserId == currentUser.UserId && b.Month == request.Month && b.Year == request.Year)
            .Select(b => new BudgetDto(
                b.Id,
                new CategoryDto(b.Category.Id, b.Category.Name, b.Category.Type, b.Category.Icon, b.Category.Color, b.Category.IsSystem),
                b.LimitAmount,
                b.SpentAmount,
                b.LimitAmount - b.SpentAmount,
                b.Month,
                b.Year,
                b.LimitAmount > 0 ? (double)b.SpentAmount / (double)b.LimitAmount * 100 : 0))
            .ToListAsync(cancellationToken);
    }
}
