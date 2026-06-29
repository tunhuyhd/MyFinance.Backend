using Microsoft.AspNetCore.Mvc;
using MyFinance.Application.SavingsGoals.Commands;
using MyFinance.Application.SavingsGoals.Dto;
using MyFinance.Application.SavingsGoals.Queries;

namespace MyFinance.WebApi.Controllers;

public class SavingsGoalsController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<SavingsGoalDto>>> GetAll()
        => await Mediator.Send(new GetSavingsGoalsQuery());

    [HttpPost]
    public async Task<ActionResult<SavingsGoalDto>> Create(CreateSavingsGoalRequest request)
        => await Mediator.Send(new CreateSavingsGoalCommand(request));

    [HttpPost("{id:guid}/contribute")]
    public async Task<ActionResult<SavingsGoalDto>> Contribute(Guid id, ContributeRequest request)
        => await Mediator.Send(new ContributeSavingsGoalCommand(id, request));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
        => await Mediator.Send(new DeleteSavingsGoalCommand(id));
}
