using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Accounts.Dto;
using MyFinance.Application.Common.Exceptions;
using MyFinance.Application.Common.Interfaces;
using MyFinance.Domain.Entities;

namespace MyFinance.Application.Accounts.Commands;

public record UpdateAccountCommand(Guid Id, UpdateAccountRequest Request) : IRequest<AccountDto>;

public class UpdateAccountCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<UpdateAccountCommand, AccountDto>
{
    public async Task<AccountDto> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await context.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Account), request.Id);

        if (request.Request.IsDefault && !account.IsDefault)
        {
            var defaultAccounts = await context.Accounts
                .Where(a => a.UserId == currentUser.UserId && a.IsDefault)
                .ToListAsync(cancellationToken);
                
            foreach (var acc in defaultAccounts)
            {
                acc.IsDefault = false;
            }
        }

        account.Name = request.Request.Name;
        account.Color = request.Request.Color;
        account.Icon = request.Request.Icon;
        account.IsDefault = request.Request.IsDefault;

        await context.SaveChangesAsync(cancellationToken);
        return new AccountDto(account.Id, account.Name, account.AccountType, account.Balance, account.Currency, account.Color, account.Icon, account.IsDefault);
    }
}
