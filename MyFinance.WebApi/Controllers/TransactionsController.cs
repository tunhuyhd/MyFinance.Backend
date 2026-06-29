using Microsoft.AspNetCore.Mvc;
using MyFinance.Application.Transactions.Commands;
using MyFinance.Application.Transactions.Dto;
using MyFinance.Application.Transactions.Queries;

namespace MyFinance.WebApi.Controllers;

public class TransactionsController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<TransactionDto>>> GetAll([FromQuery] TransactionFilterRequest filter)
        => await Mediator.Send(new GetTransactionsQuery(filter));

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create(CreateTransactionRequest request)
        => await Mediator.Send(new CreateTransactionCommand(request));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
        => await Mediator.Send(new DeleteTransactionCommand(id));
}
