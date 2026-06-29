using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Common.Interfaces;
using MyFinance.Application.Transactions.Dto;

namespace MyFinance.Application.Transactions.Queries;

public record GetTransactionsQuery(TransactionFilterRequest Filter) : IRequest<PagedResult<TransactionDto>>;

public class GetTransactionsQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetTransactionsQuery, PagedResult<TransactionDto>>
{
    public async Task<PagedResult<TransactionDto>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        var userId = currentUser.UserId!.Value;

        var query = context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.ToAccount)
            .Where(t => t.UserId == userId);

        if (filter.AccountId.HasValue)
            query = query.Where(t => t.AccountId == filter.AccountId);

        if (filter.CategoryId.HasValue)
            query = query.Where(t => t.CategoryId == filter.CategoryId);

        if (filter.Type.HasValue)
            query = query.Where(t => t.Type == filter.Type);

        if (filter.From.HasValue)
            query = query.Where(t => t.TransactionDate >= filter.From);

        if (filter.To.HasValue)
            query = query.Where(t => t.TransactionDate <= filter.To);

        if (!string.IsNullOrEmpty(filter.Search))
            query = query.Where(t => t.Description.Contains(filter.Search));

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.TransactionDate)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(t => new TransactionDto(
                t.Id, t.AccountId, t.Account.Name,
                t.CategoryId, t.Category != null ? t.Category.Name : null,
                t.Category != null ? t.Category.Icon : null,
                t.Category != null ? t.Category.Color : null,
                t.Amount, t.Type, t.Description, t.Note, t.TransactionDate,
                t.ToAccountId, t.ToAccount != null ? t.ToAccount.Name : null))
            .ToListAsync(cancellationToken);

        return new PagedResult<TransactionDto>(items, total, filter.Page, filter.PageSize);
    }
}
