using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinance.Application.Auth.Commands;
using MyFinance.Application.Auth.Dto;
using MyFinance.Application.Auth.Queries;

namespace MyFinance.WebApi.Controllers;

public class AuthController : BaseApiController
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        => await Mediator.Send(new LoginCommand(request.Username, request.Password));

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        => await Mediator.Send(new RegisterCommand(request));

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> RefreshToken(RefreshTokenRequest request)
        => await Mediator.Send(new RefreshTokenCommand(request));

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe()
        => await Mediator.Send(new GetCurrentUserQuery());
}
