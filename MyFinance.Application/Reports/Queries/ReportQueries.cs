using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Common.Interfaces;
using MyFinance.Domain.Enums;

namespace MyFinance.Application.Reports.Queries;

// Dashboard Summary
public record DashboardSummaryDto(
    decimal TotalBalance,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal NetSavings,
    int Month,
    int Year,
    List<CategorySummaryDto> TopExpenseCategories,
    List<DailyTrendDto> DailyTrend);

public record CategorySummaryDto(string CategoryName, string? CategoryColor, string? CategoryIcon, decimal Amount, double Percentage);
public record DailyTrendDto(string Date, decimal Income, decimal Expense);
public record MonthlyReportDto(int Month, int Year, decimal Income, decimal Expense, decimal Net);

public record GetDashboardSummaryQuery(int Month, int Year) : IRequest<DashboardSummaryDto>;

public class GetDashboardSummaryQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;
        var from = new DateTime(request.Year, request.Month, 1);
        var to = from.AddMonths(1).AddDays(-1);

        // Total balance across all accounts
        var totalBalance = await context.Accounts
            .Where(a => a.UserId == userId)
            .SumAsync(a => a.Balance, cancellationToken);

        // Monthly income/expense
        var monthTransactions = await context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && t.TransactionDate >= from && t.TransactionDate <= to)
            .ToListAsync(cancellationToken);

        var totalIncome = monthTransactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var totalExpense = monthTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        // Top expense categories
        var expenseByCategory = monthTransactions
            .Where(t => t.Type == TransactionType.Expense && t.Category != null)
            .GroupBy(t => new { t.Category!.Name, t.Category.Color, t.Category.Icon })
            .Select(g => new { g.Key.Name, g.Key.Color, g.Key.Icon, Total = g.Sum(x => x.Amount) })
            .OrderByDescending(x => x.Total)
            .Take(5)
            .ToList();

        var topCategories = expenseByCategory.Select(c => new CategorySummaryDto(
            c.Name, c.Color, c.Icon, c.Total,
            totalExpense > 0 ? (double)c.Total / (double)totalExpense * 100 : 0)).ToList();

        // Daily trend
        var dailyTrend = monthTransactions
            .Where(t => t.Type != TransactionType.Transfer)
            .GroupBy(t => t.TransactionDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => new DailyTrendDto(
                g.Key.ToString("yyyy-MM-dd"),
                g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)))
            .ToList();

        return new DashboardSummaryDto(totalBalance, totalIncome, totalExpense, totalIncome - totalExpense,
            request.Month, request.Year, topCategories, dailyTrend);
    }
}

// Monthly Report (last 12 months)
public record GetMonthlyReportQuery : IRequest<List<MonthlyReportDto>>;

public class GetMonthlyReportQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetMonthlyReportQuery, List<MonthlyReportDto>>
{
    public async Task<List<MonthlyReportDto>> Handle(GetMonthlyReportQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;
        var from = DateTime.UtcNow.AddMonths(-11);
        from = new DateTime(from.Year, from.Month, 1);

        var transactions = await context.Transactions
            .Where(t => t.UserId == userId && t.TransactionDate >= from && t.Type != TransactionType.Transfer)
            .ToListAsync(cancellationToken);

        return transactions
            .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlyReportDto(
                g.Key.Month, g.Key.Year,
                g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount) - g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)))
            .ToList();
    }
}
