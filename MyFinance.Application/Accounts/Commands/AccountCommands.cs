using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Accounts.Dto;
using MyFinance.Application.Common.Exceptions;
using MyFinance.Application.Common.Interfaces;
using MyFinance.Domain.Entities;

namespace MyFinance.Application.Accounts.Commands;

// CREATE
public record CreateAccountCommand(CreateAccountRequest Request) : IRequest<AccountDto>;

public class CreateAccountCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<CreateAccountCommand, AccountDto>
{
    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var userId = currentUser.UserId!.Value;

        if (req.IsDefault)
        {
            var defaultAccounts = await context.Accounts
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync(cancellationToken);
                
            foreach (var acc in defaultAccounts)
            {
                acc.IsDefault = false;
            }
        }

        var account = new Account
        {
            UserId = userId,
            Name = req.Name,
            AccountType = req.AccountType,
            Balance = req.InitialBalance,
            Currency = req.Currency,
            Color = req.Color,
            Icon = req.Icon,
            IsDefault = req.IsDefault
        };

        await context.Accounts.AddAsync(account, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return ToDto(account);
    }

    private static AccountDto ToDto(Account a) => new(a.Id, a.Name, a.AccountType, a.Balance, a.Currency, a.Color, a.Icon, a.IsDefault);
}

// UPDATE
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

        account.Name = request.Request.Name;
        account.Color = request.Request.Color;
        account.Icon = request.Request.Icon;

        await context.SaveChangesAsync(cancellationToken);
        return new AccountDto(account.Id, account.Name, account.AccountType, account.Balance, account.Currency, account.Color, account.Icon, account.IsDefault);
    }
}

// DELETE
public record DeleteAccountCommand(Guid Id) : IRequest<bool>;

public class DeleteAccountCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<DeleteAccountCommand, bool>
{
    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await context.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Account), request.Id);

        context.Accounts.Remove(account);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
