using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Auth.Dto;
using MyFinance.Application.Common.Interfaces;

namespace MyFinance.Application.Auth.Commands;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<AuthResponse>;

public class RefreshTokenCommandHandler(
    IApplicationDbContext context,
    IJwtService jwtService) : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.Request.RefreshToken, cancellationToken);

        if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        var newToken = jwtService.GenerateToken(user);
        var newRefreshToken = jwtService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await context.SaveChangesAsync(cancellationToken);

        return new AuthResponse(newToken, newRefreshToken, user.Username, user.Id);
    }
}
