using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Common.Interfaces;

namespace MyFinance.Application.Auth.Commands;

public record DeleteUserCommand(Guid UserId) : IRequest<bool>;

public class DeleteUserCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            return false;

        // Xóa cứng User. 
        // Note: Cascade delete sẽ tự động xóa các Accounts, Categories, Transactions, Budgets, SavingsGoals liên quan
        // (Dựa trên cấu hình trong ApplicationDbContext.cs)
        context.Users.Remove(user);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
