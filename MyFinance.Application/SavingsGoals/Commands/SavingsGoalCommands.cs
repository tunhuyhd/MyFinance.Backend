using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Common.Exceptions;
using MyFinance.Application.Common.Interfaces;
using MyFinance.Application.SavingsGoals.Dto;
using MyFinance.Domain.Entities;
using MyFinance.Domain.Enums;

namespace MyFinance.Application.SavingsGoals.Commands;

public record CreateSavingsGoalCommand(CreateSavingsGoalRequest Request) : IRequest<SavingsGoalDto>;

public class CreateSavingsGoalCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<CreateSavingsGoalCommand, SavingsGoalDto>
{
    public async Task<SavingsGoalDto> Handle(CreateSavingsGoalCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var goal = new SavingsGoal
        {
            UserId = currentUser.UserId!.Value,
            Name = req.Name,
            TargetAmount = req.TargetAmount,
            CurrentAmount = 0,
            Deadline = req.Deadline,
            Icon = req.Icon,
            Color = req.Color
        };

        await context.SavingsGoals.AddAsync(goal, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return ToDto(goal);
    }

    private static SavingsGoalDto ToDto(SavingsGoal g)
    {
        var remaining = g.TargetAmount - g.CurrentAmount;
        var pct = g.TargetAmount > 0 ? (double)g.CurrentAmount / (double)g.TargetAmount * 100 : 0;
        return new SavingsGoalDto(g.Id, g.Name, g.TargetAmount, g.CurrentAmount, remaining, pct, g.Deadline, g.Icon, g.Color, g.Status);
    }
}

public record ContributeSavingsGoalCommand(Guid Id, ContributeRequest Request) : IRequest<SavingsGoalDto>;

public class ContributeSavingsGoalCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<ContributeSavingsGoalCommand, SavingsGoalDto>
{
    public async Task<SavingsGoalDto> Handle(ContributeSavingsGoalCommand request, CancellationToken cancellationToken)
    {
        var goal = await context.SavingsGoals
            .FirstOrDefaultAsync(g => g.Id == request.Id && g.UserId == currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(SavingsGoal), request.Id);

        goal.CurrentAmount += request.Request.Amount;
        if (goal.CurrentAmount >= goal.TargetAmount)
            goal.Status = SavingsGoalStatus.Completed;

        await context.SaveChangesAsync(cancellationToken);

        var remaining = goal.TargetAmount - goal.CurrentAmount;
        var pct = goal.TargetAmount > 0 ? (double)goal.CurrentAmount / (double)goal.TargetAmount * 100 : 0;
        return new SavingsGoalDto(goal.Id, goal.Name, goal.TargetAmount, goal.CurrentAmount, remaining, pct, goal.Deadline, goal.Icon, goal.Color, goal.Status);
    }
}

public record DeleteSavingsGoalCommand(Guid Id) : IRequest<bool>;

public class DeleteSavingsGoalCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<DeleteSavingsGoalCommand, bool>
{
    public async Task<bool> Handle(DeleteSavingsGoalCommand request, CancellationToken cancellationToken)
    {
        var goal = await context.SavingsGoals
            .FirstOrDefaultAsync(g => g.Id == request.Id && g.UserId == currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(SavingsGoal), request.Id);

        context.SavingsGoals.Remove(goal);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
