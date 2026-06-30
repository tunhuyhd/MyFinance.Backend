using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Auth.Dto;
using MyFinance.Application.Common.Interfaces;

namespace MyFinance.Application.Auth.Queries;

public record GetUsersQuery : IRequest<List<UserDto>>;

public class GetUsersQueryHandler(IApplicationDbContext context) : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await context.Users
            .OrderByDescending(u => u.CreatedOn)
            .Select(u => new UserDto(u.Id, u.Username, u.Email, u.FullName, u.AvatarUrl, u.IsAdmin))
            .ToListAsync(cancellationToken);

        return users;
    }
}
