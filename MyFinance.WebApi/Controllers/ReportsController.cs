using Microsoft.AspNetCore.Mvc;
using MyFinance.Application.Reports.Queries;

namespace MyFinance.WebApi.Controllers;

public class ReportsController : BaseApiController
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardSummaryDto>> GetDashboard([FromQuery] int? month, [FromQuery] int? year)
    {
        var now = DateTime.UtcNow;
        return await Mediator.Send(new GetDashboardSummaryQuery(month ?? now.Month, year ?? now.Year));
    }

    [HttpGet("monthly")]
    public async Task<ActionResult<List<MonthlyReportDto>>> GetMonthly()
        => await Mediator.Send(new GetMonthlyReportQuery());

    [HttpGet("category-expenses")]
    public async Task<ActionResult<List<CategorySummaryDto>>> GetCategoryExpenses([FromQuery] int month, [FromQuery] int year)
        => await Mediator.Send(new GetCategoryExpenseReportQuery(month, year));
}
