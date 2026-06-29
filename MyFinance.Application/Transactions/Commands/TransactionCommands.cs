using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Common.Exceptions;
using MyFinance.Application.Common.Interfaces;
using MyFinance.Application.Transactions.Dto;
using MyFinance.Domain.Entities;
using MyFinance.Domain.Enums;

namespace MyFinance.Application.Transactions.Commands;

// CREATE
public record CreateTransactionCommand(CreateTransactionRequest Request) : IRequest<TransactionDto>;

public class CreateTransactionCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var userId = currentUser.UserId!.Value;

        var account = await context.Accounts
            .FirstOrDefaultAsync(a => a.Id == req.AccountId && a.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Account), req.AccountId);

        // Update balance
        if (req.Type == TransactionType.Income)
            account.Balance += req.Amount;
        else if (req.Type == TransactionType.Expense)
            account.Balance -= req.Amount;
        else if (req.Type == TransactionType.Transfer && req.ToAccountId.HasValue)
        {
            account.Balance -= req.Amount;
            var toAccount = await context.Accounts
                .FirstOrDefaultAsync(a => a.Id == req.ToAccountId && a.UserId == userId, cancellationToken)
                ?? throw new NotFoundException(nameof(Account), req.ToAccountId);
            toAccount.Balance += req.Amount;
        }

        // Update budget spent amount
        if (req.Type == TransactionType.Expense && req.CategoryId.HasValue)
        {
            var now = req.TransactionDate;
            var budget = await context.Budgets
                .FirstOrDefaultAsync(b => b.UserId == userId
                    && b.CategoryId == req.CategoryId
                    && b.Month == now.Month
                    && b.Year == now.Year, cancellationToken);

            if (budget != null)
                budget.SpentAmount += req.Amount;
        }

        var transaction = new Transaction
        {
            UserId = userId,
            AccountId = req.AccountId,
            CategoryId = req.CategoryId,
            Amount = req.Amount,
            Type = req.Type,
            Description = req.Description,
            Note = req.Note,
            TransactionDate = req.TransactionDate,
            ToAccountId = req.ToAccountId
        };

        await context.Transactions.AddAsync(transaction, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return await BuildDto(transaction, context, cancellationToken);
    }

    private static async Task<TransactionDto> BuildDto(Transaction t, IApplicationDbContext ctx, CancellationToken ct)
    {
        var account = await ctx.Accounts.FindAsync([t.AccountId], ct);
        Category? category = t.CategoryId.HasValue ? await ctx.Categories.FindAsync([t.CategoryId.Value], ct) : null;
        Account? toAccount = t.ToAccountId.HasValue ? await ctx.Accounts.FindAsync([t.ToAccountId.Value], ct) : null;

        return new TransactionDto(t.Id, t.AccountId, account?.Name ?? "", t.CategoryId, category?.Name,
            category?.Icon, category?.Color, t.Amount, t.Type, t.Description, t.Note,
            t.TransactionDate, t.ToAccountId, toAccount?.Name);
    }
}

// DELETE
public record DeleteTransactionCommand(Guid Id) : IRequest<bool>;

public class DeleteTransactionCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<DeleteTransactionCommand, bool>
{
    public async Task<bool> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await context.Transactions
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Transaction), request.Id);

        // Reverse balance
        var account = await context.Accounts.FindAsync([transaction.AccountId], cancellationToken);
        if (account != null)
        {
            if (transaction.Type == TransactionType.Income) account.Balance -= transaction.Amount;
            else if (transaction.Type == TransactionType.Expense) account.Balance += transaction.Amount;
            else if (transaction.Type == TransactionType.Transfer && transaction.ToAccountId.HasValue)
            {
                account.Balance += transaction.Amount;
                var toAccount = await context.Accounts.FindAsync([transaction.ToAccountId.Value], cancellationToken);
                if (toAccount != null) toAccount.Balance -= transaction.Amount;
            }
        }

        context.Transactions.Remove(transaction);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
