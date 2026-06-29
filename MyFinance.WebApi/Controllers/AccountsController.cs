using Microsoft.AspNetCore.Mvc;
using MyFinance.Application.Accounts.Commands;
using MyFinance.Application.Accounts.Dto;
using MyFinance.Application.Accounts.Queries;

namespace MyFinance.WebApi.Controllers;

public class AccountsController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<AccountDto>>> GetAll()
        => await Mediator.Send(new GetAccountsQuery());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccountDto>> GetById(Guid id)
        => await Mediator.Send(new GetAccountByIdQuery(id));

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create(CreateAccountRequest request)
        => await Mediator.Send(new CreateAccountCommand(request));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AccountDto>> Update(Guid id, UpdateAccountRequest request)
        => await Mediator.Send(new UpdateAccountCommand(id, request));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
        => await Mediator.Send(new DeleteAccountCommand(id));
}
