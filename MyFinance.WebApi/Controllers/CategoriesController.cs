using Microsoft.AspNetCore.Mvc;
using MyFinance.Application.Categories.Commands;
using MyFinance.Application.Categories.Dto;
using MyFinance.Application.Categories.Queries;

namespace MyFinance.WebApi.Controllers;

public class CategoriesController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll([FromQuery] string? type)
        => await Mediator.Send(new GetCategoriesQuery(type));

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryRequest request)
        => await Mediator.Send(new CreateCategoryCommand(request));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> Update(Guid id, UpdateCategoryRequest request)
        => await Mediator.Send(new UpdateCategoryCommand(id, request));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
        => await Mediator.Send(new DeleteCategoryCommand(id));
}
