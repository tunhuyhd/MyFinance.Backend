using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Common.Interfaces;
using MyFinance.Domain.Common;
using MyFinance.Domain.Entities;
using MyFinance.Domain.Enums;

namespace MyFinance.Infrastructure.Persistence.Context;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<SavingsGoal> SavingsGoals => Set<SavingsGoal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Account
        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(e => e.Balance).HasPrecision(18, 2);
            entity.HasOne(a => a.User).WithMany(u => u.Accounts)
                .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Category - nullable UserId for system categories
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasOne(c => c.User).WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        });

        // Transaction
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.HasOne(t => t.Account).WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(t => t.Category).WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
            entity.HasOne(t => t.ToAccount).WithMany()
                .HasForeignKey(t => t.ToAccountId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
        });

        // Budget
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.Property(e => e.LimitAmount).HasPrecision(18, 2);
            entity.Property(e => e.SpentAmount).HasPrecision(18, 2);
            entity.HasOne(b => b.Category).WithMany(c => c.Budgets)
                .HasForeignKey(b => b.CategoryId).OnDelete(DeleteBehavior.Cascade);
        });

        // SavingsGoal
        modelBuilder.Entity<SavingsGoal>(entity =>
        {
            entity.Property(e => e.TargetAmount).HasPrecision(18, 2);
            entity.Property(e => e.CurrentAmount).HasPrecision(18, 2);
        });

        // Seed system categories
        SeedSystemCategories(modelBuilder);
        
        // Seed Admin user
        var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = adminId,
            Username = "admin",
            Email = "admin@myfinance.local",
            PasswordHash = "$2a$11$KvCWuCbUfdprIh5NNvPOYuNRDBiLiBRbwJ3BfFemiNhF7aFXQWbmC",
            FullName = "System Administrator",
            IsAdmin = true,
            CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy = Guid.Empty
        });

        // UTC DateTime converter for PostgreSQL
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.GetProperties()
                .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

            foreach (var property in properties)
            {
                property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                    v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                    v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc)));
            }
        }
    }

    private static void SeedSystemCategories(ModelBuilder modelBuilder)
    {
        var fixedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var categories = new[]
        {
            // Expense categories
            new Category { Id = Guid.Parse("10000001-0000-0000-0000-000000000001"), Name = "Ăn uống", Type = CategoryType.Expense, Icon = "utensils", Color = "#F97316", IsSystem = true, CreatedBy = Guid.Empty, CreatedOn = fixedDate },
            new Category { Id = Guid.Parse("10000001-0000-0000-0000-000000000002"), Name = "Di chuyển", Type = CategoryType.Expense, Icon = "car", Color = "#3B82F6", IsSystem = true, CreatedBy = Guid.Empty, CreatedOn = fixedDate },
            new Category { Id = Guid.Parse("10000001-0000-0000-0000-000000000003"), Name = "Mua sắm", Type = CategoryType.Expense, Icon = "shopping-bag", Color = "#EC4899", IsSystem = true, CreatedBy = Guid.Empty, CreatedOn = fixedDate },
            new Category { Id = Guid.Parse("10000001-0000-0000-0000-000000000004"), Name = "Giải trí", Type = CategoryType.Expense, Icon = "gamepad-2", Color = "#8B5CF6", IsSystem = true, CreatedBy = Guid.Empty, CreatedOn = fixedDate },
            new Category { Id = Guid.Parse("10000001-0000-0000-0000-000000000005"), Name = "Sức khỏe", Type = CategoryType.Expense, Icon = "heart-pulse", Color = "#EF4444", IsSystem = true, CreatedBy = Guid.Empty, CreatedOn = fixedDate },
            new Category { Id = Guid.Parse("10000001-0000-0000-0000-000000000006"), Name = "Giáo dục", Type = CategoryType.Expense, Icon = "book-open", Color = "#06B6D4", IsSystem = true, CreatedBy = Guid.Empty, CreatedOn = fixedDate },
            new Category { Id = Guid.Parse("10000001-0000-0000-0000-000000000007"), Name = "Hóa đơn", Type = CategoryType.Expense, Icon = "receipt", Color = "#64748B", IsSystem = true, CreatedBy = Guid.Empty, CreatedOn = fixedDate },
            new Category { Id = Guid.Parse("10000001-0000-0000-0000-000000000008"), Name = "Khác", Type = CategoryType.Expense, Icon = "more-horizontal", Color = "#6B7280", IsSystem = true, CreatedBy = Guid.Empty, CreatedOn = fixedDate },
            new Category { Id = Guid.Parse("10000001-0000-0000-0000-000000000009"), Name = "Rút tiền", Type = CategoryType.Expense, Icon = "banknote", Color = "#14B8A6", IsSystem = true, CreatedBy = Guid.Empty, CreatedOn = fixedDate },
            // Income categories
            new Category { Id = Guid.Parse("20000001-0000-0000-0000-000000000001"), Name = "Lương", Type = CategoryType.Income, Icon = "briefcase", Color = "#10B981", IsSystem = true, CreatedBy = Guid.Empty, CreatedOn = fixedDate },
            new Category { Id = Guid.Parse("20000001-0000-0000-0000-000000000002"), Name = "Đầu tư", Type = CategoryType.Income, Icon = "trending-up", Color = "#059669", IsSystem = true, CreatedBy = Guid.Empty, CreatedOn = fixedDate },
            new Category { Id = Guid.Parse("20000001-0000-0000-0000-000000000003"), Name = "Kinh doanh", Type = CategoryType.Income, Icon = "store", Color = "#0EA5E9", IsSystem = true, CreatedBy = Guid.Empty, CreatedOn = fixedDate },
            new Category { Id = Guid.Parse("20000001-0000-0000-0000-000000000004"), Name = "Quà tặng", Type = CategoryType.Income, Icon = "gift", Color = "#F59E0B", IsSystem = true, CreatedBy = Guid.Empty, CreatedOn = fixedDate },
            new Category { Id = Guid.Parse("20000001-0000-0000-0000-000000000005"), Name = "Thu nhập khác", Type = CategoryType.Income, Icon = "plus-circle", Color = "#22C55E", IsSystem = true, CreatedBy = Guid.Empty, CreatedOn = fixedDate },
        };

        modelBuilder.Entity<Category>().HasData(categories);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case Microsoft.EntityFrameworkCore.EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedOn = now;
                    break;
                case Microsoft.EntityFrameworkCore.EntityState.Modified:
                    entry.Entity.LastModifiedBy = userId;
                    entry.Entity.LastModifiedOn = now;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
