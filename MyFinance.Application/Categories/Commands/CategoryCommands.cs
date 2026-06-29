using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Categories.Dto;
using MyFinance.Application.Common.Exceptions;
using MyFinance.Application.Common.Interfaces;
using MyFinance.Domain.Entities;

namespace MyFinance.Application.Categories.Commands;

// CREATE
public record CreateCategoryCommand(CreateCategoryRequest Request) : IRequest<CategoryDto>;

public class CreateCategoryCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            UserId = currentUser.UserId,
            Name = request.Request.Name,
            Type = request.Request.Type,
            Icon = request.Request.Icon,
            Color = request.Request.Color,
            IsSystem = false
        };

        await context.Categories.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return ToDto(category);
    }

    private static CategoryDto ToDto(Category c) => new(c.Id, c.Name, c.Type, c.Icon, c.Color, c.IsSystem);
}

// UPDATE
public record UpdateCategoryCommand(Guid Id, UpdateCategoryRequest Request) : IRequest<CategoryDto>;

public class UpdateCategoryCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == currentUser.UserId && !c.IsSystem, cancellationToken)
            ?? throw new NotFoundException(nameof(Category), request.Id);

        category.Name = request.Request.Name;
        category.Icon = request.Request.Icon;
        category.Color = request.Request.Color;

        await context.SaveChangesAsync(cancellationToken);
        return new CategoryDto(category.Id, category.Name, category.Type, category.Icon, category.Color, category.IsSystem);
    }
}

// DELETE
public record DeleteCategoryCommand(Guid Id) : IRequest<bool>;

public class DeleteCategoryCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<DeleteCategoryCommand, bool>
{
    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == currentUser.UserId && !c.IsSystem, cancellationToken)
            ?? throw new NotFoundException(nameof(Category), request.Id);

        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
