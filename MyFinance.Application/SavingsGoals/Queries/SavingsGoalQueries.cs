using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Common.Interfaces;
using MyFinance.Application.SavingsGoals.Dto;

namespace MyFinance.Application.SavingsGoals.Queries;

public record GetSavingsGoalsQuery : IRequest<List<SavingsGoalDto>>;

public class GetSavingsGoalsQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetSavingsGoalsQuery, List<SavingsGoalDto>>
{
    public async Task<List<SavingsGoalDto>> Handle(GetSavingsGoalsQuery request, CancellationToken cancellationToken)
    {
        return await context.SavingsGoals
            .Where(g => g.UserId == currentUser.UserId)
            .OrderBy(g => g.Status)
            .ThenByDescending(g => g.CreatedOn)
            .Select(g => new SavingsGoalDto(
                g.Id, g.Name, g.TargetAmount, g.CurrentAmount,
                g.TargetAmount - g.CurrentAmount,
                g.TargetAmount > 0 ? (double)g.CurrentAmount / (double)g.TargetAmount * 100 : 0,
                g.Deadline, g.Icon, g.Color, g.Status))
            .ToListAsync(cancellationToken);
    }
}
