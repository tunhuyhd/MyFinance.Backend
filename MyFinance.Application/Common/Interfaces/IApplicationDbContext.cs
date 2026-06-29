using Microsoft.EntityFrameworkCore;
using MyFinance.Domain.Entities;

namespace MyFinance.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Account> Accounts { get; }
    DbSet<Category> Categories { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<Budget> Budgets { get; }
    DbSet<SavingsGoal> SavingsGoals { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
