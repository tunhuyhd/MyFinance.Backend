using MyFinance.Domain.Enums;

namespace MyFinance.Application.Categories.Dto;

public record CategoryDto(
    Guid Id,
    string Name,
    CategoryType Type,
    string Icon,
    string Color,
    bool IsSystem);

public record CreateCategoryRequest(string Name, CategoryType Type, string Icon, string Color);
public record UpdateCategoryRequest(string Name, string Icon, string Color);
