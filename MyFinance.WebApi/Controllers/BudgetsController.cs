using Microsoft.AspNetCore.Mvc;
using MyFinance.Application.Budgets.Commands;
using MyFinance.Application.Budgets.Dto;
using MyFinance.Application.Budgets.Queries;

namespace MyFinance.WebApi.Controllers;

public class BudgetsController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<BudgetDto>>> GetAll([FromQuery] int month, [FromQuery] int year)
        => await Mediator.Send(new GetBudgetsQuery(month, year));

    [HttpPost]
    public async Task<ActionResult<BudgetDto>> Create(CreateBudgetRequest request)
        => await Mediator.Send(new CreateBudgetCommand(request));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BudgetDto>> Update(Guid id, UpdateBudgetRequest request)
        => await Mediator.Send(new UpdateBudgetCommand(id, request));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
        => await Mediator.Send(new DeleteBudgetCommand(id));

    [HttpPost("copy")]
    public async Task<ActionResult<bool>> CopyPreviousMonth([FromQuery] int targetMonth, [FromQuery] int targetYear)
        => await Mediator.Send(new CopyPreviousMonthBudgetsCommand(targetMonth, targetYear));
}
