using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Categories.Dto;
using MyFinance.Application.Common.Interfaces;

namespace MyFinance.Application.Categories.Queries;

public record GetCategoriesQuery(string? Type = null) : IRequest<List<CategoryDto>>;

public class GetCategoriesQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = context.Categories
            .Where(c => c.IsSystem || c.UserId == currentUser.UserId);

        if (!string.IsNullOrEmpty(request.Type) && Enum.TryParse<Domain.Enums.CategoryType>(request.Type, out var catType))
            query = query.Where(c => c.Type == catType);

        return await query
            .OrderBy(c => c.IsSystem ? 0 : 1)
            .ThenBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Type, c.Icon, c.Color, c.IsSystem))
            .ToListAsync(cancellationToken);
    }
}
