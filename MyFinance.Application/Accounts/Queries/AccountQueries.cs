using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Accounts.Dto;
using MyFinance.Application.Common.Exceptions;
using MyFinance.Application.Common.Interfaces;

namespace MyFinance.Application.Accounts.Queries;

public record GetAccountsQuery : IRequest<List<AccountDto>>;

public class GetAccountsQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetAccountsQuery, List<AccountDto>>
{
    public async Task<List<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        return await context.Accounts
            .Where(a => a.UserId == currentUser.UserId)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.Name)
            .Select(a => new AccountDto(a.Id, a.Name, a.AccountType, a.Balance, a.Currency, a.Color, a.Icon, a.IsDefault))
            .ToListAsync(cancellationToken);
    }
}

public record GetAccountByIdQuery(Guid Id) : IRequest<AccountDto>;

public class GetAccountByIdQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetAccountByIdQuery, AccountDto>
{
    public async Task<AccountDto> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await context.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Account), request.Id);

        return new AccountDto(account.Id, account.Name, account.AccountType, account.Balance, account.Currency, account.Color, account.Icon, account.IsDefault);
    }
}
