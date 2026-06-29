using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Common.Exceptions;
using MyFinance.Application.Common.Interfaces;
using MyFinance.Domain.Entities;

namespace MyFinance.Application.Accounts.Commands;

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
