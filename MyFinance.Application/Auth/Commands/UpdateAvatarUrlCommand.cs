using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Auth.Dto;
using MyFinance.Application.Common.Exceptions;
using MyFinance.Application.Common.Interfaces;

namespace MyFinance.Application.Auth.Commands;

public record UpdateAvatarUrlCommand(string AvatarUrl) : IRequest<UserDto>;

public class UpdateAvatarUrlCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<UpdateAvatarUrlCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateAvatarUrlCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), currentUser.UserId!);

        user.AvatarUrl = request.AvatarUrl;
        await context.SaveChangesAsync(cancellationToken);

        return new UserDto(user.Id, user.Username, user.Email, user.FullName, user.AvatarUrl, user.IsAdmin);
    }
}
