using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Budgets.Dto;
using MyFinance.Application.Categories.Dto;
using MyFinance.Application.Common.Exceptions;
using MyFinance.Application.Common.Interfaces;
using MyFinance.Domain.Entities;

namespace MyFinance.Application.Budgets.Commands;

public record CreateBudgetCommand(CreateBudgetRequest Request) : IRequest<BudgetDto>;

public class CreateBudgetCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<CreateBudgetCommand, BudgetDto>
{
    public async Task<BudgetDto> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var userId = currentUser.UserId!.Value;

        var exists = await context.Budgets
            .AnyAsync(b => b.UserId == userId && b.CategoryId == req.CategoryId
                && b.Month == req.Month && b.Year == req.Year, cancellationToken);

        if (exists) throw new ConflictException("Budget for this category and month already exists.");

        var budget = new Budget
        {
            UserId = userId,
            CategoryId = req.CategoryId,
            LimitAmount = req.LimitAmount,
            SpentAmount = 0,
            Month = req.Month,
            Year = req.Year
        };

        await context.Budgets.AddAsync(budget, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Real-time calculation of SpentAmount from Transactions
        var spentAmount = await context.Transactions
            .Where(t => t.AccountId != null && t.CategoryId == budget.CategoryId 
                   && t.TransactionDate.Month == budget.Month 
                   && t.TransactionDate.Year == budget.Year 
                   && t.Type == Domain.Enums.TransactionType.Expense)
            .SumAsync(t => t.Amount, cancellationToken);

        var category = await context.Categories.FindAsync([budget.CategoryId], cancellationToken);
        var catDto = new CategoryDto(category!.Id, category.Name, category.Type, category.Icon, category.Color, category.IsSystem);
        var remaining = budget.LimitAmount - spentAmount;
        var pct = budget.LimitAmount > 0 ? (double)spentAmount / (double)budget.LimitAmount * 100 : 0;

        return new BudgetDto(budget.Id, catDto, budget.LimitAmount, spentAmount, remaining, budget.Month, budget.Year, pct);
    }
}

public record UpdateBudgetCommand(Guid Id, UpdateBudgetRequest Request) : IRequest<BudgetDto>;

public class UpdateBudgetCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<UpdateBudgetCommand, BudgetDto>
{
    public async Task<BudgetDto> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await context.Budgets
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.UserId == currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Budget), request.Id);

        budget.LimitAmount = request.Request.LimitAmount;
        await context.SaveChangesAsync(cancellationToken);

        var catDto = new CategoryDto(budget.Category.Id, budget.Category.Name, budget.Category.Type, budget.Category.Icon, budget.Category.Color, budget.Category.IsSystem);
        
        // Real-time calculation of SpentAmount from Transactions
        var spentAmount = await context.Transactions
            .Where(t => t.AccountId != null && t.CategoryId == budget.CategoryId 
                   && t.TransactionDate.Month == budget.Month 
                   && t.TransactionDate.Year == budget.Year 
                   && t.Type == Domain.Enums.TransactionType.Expense)
            .SumAsync(t => t.Amount, cancellationToken);
            
        var remaining = budget.LimitAmount - spentAmount;
        var pct = budget.LimitAmount > 0 ? (double)spentAmount / (double)budget.LimitAmount * 100 : 0;
        return new BudgetDto(budget.Id, catDto, budget.LimitAmount, spentAmount, remaining, budget.Month, budget.Year, pct);
    }
}

public record DeleteBudgetCommand(Guid Id) : IRequest<bool>;

public class DeleteBudgetCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<DeleteBudgetCommand, bool>
{
    public async Task<bool> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await context.Budgets
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.UserId == currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Budget), request.Id);

        context.Budgets.Remove(budget);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record CopyPreviousMonthBudgetsCommand(int TargetMonth, int TargetYear) : IRequest<bool>;

public class CopyPreviousMonthBudgetsCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<CopyPreviousMonthBudgetsCommand, bool>
{
    public async Task<bool> Handle(CopyPreviousMonthBudgetsCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;
        
        int prevMonth = request.TargetMonth == 1 ? 12 : request.TargetMonth - 1;
        int prevYear = request.TargetMonth == 1 ? request.TargetYear - 1 : request.TargetYear;

        var prevBudgets = await context.Budgets
            .Where(b => b.UserId == userId && b.Month == prevMonth && b.Year == prevYear)
            .ToListAsync(cancellationToken);

        if (!prevBudgets.Any()) return false;

        var currentBudgets = await context.Budgets
            .Where(b => b.UserId == userId && b.Month == request.TargetMonth && b.Year == request.TargetYear)
            .Select(b => b.CategoryId)
            .ToListAsync(cancellationToken);

        var newBudgets = prevBudgets
            .Where(b => !currentBudgets.Contains(b.CategoryId))
            .Select(b => new Budget
            {
                UserId = userId,
                CategoryId = b.CategoryId,
                LimitAmount = b.LimitAmount,
                SpentAmount = 0,
                Month = request.TargetMonth,
                Year = request.TargetYear
            }).ToList();

        if (newBudgets.Any())
        {
            await context.Budgets.AddRangeAsync(newBudgets, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}

