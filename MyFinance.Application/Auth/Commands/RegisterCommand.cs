using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Auth.Dto;
using MyFinance.Application.Common.Exceptions;
using MyFinance.Application.Common.Interfaces;
using MyFinance.Domain.Entities;

namespace MyFinance.Application.Auth.Commands;

public record RegisterCommand(RegisterRequest Request) : IRequest<AuthResponse>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Request.Username).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Request.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Request.FullName).NotEmpty();
    }
}

public class RegisterCommandHandler(
    IApplicationDbContext context,
    IJwtService jwtService,
    IPasswordHasher passwordHasher) : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        var exists = await context.Users
            .AnyAsync(u => u.Username == req.Username || u.Email == req.Email, cancellationToken);

        if (exists)
            throw new ConflictException("Username or email already exists.");

        var user = new User
        {
            Username = req.Username,
            Email = req.Email,
            FullName = req.FullName,
            PasswordHash = passwordHasher.Hash(req.Password)
        };

        await context.Users.AddAsync(user, cancellationToken);

        // Create default Cash account
        var defaultAccount = new Account
        {
            UserId = user.Id,
            Name = "Tiền mặt",
            AccountType = Domain.Enums.AccountType.Cash,
            Balance = 0,
            Currency = "VND",
            IsDefault = true
        };
        await context.Accounts.AddAsync(defaultAccount, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        var token = jwtService.GenerateToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        await context.SaveChangesAsync(cancellationToken);

        return new AuthResponse(token, refreshToken, user.Username, user.Id);
    }
}
