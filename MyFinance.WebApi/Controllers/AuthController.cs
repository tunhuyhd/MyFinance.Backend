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

    [HttpPost("avatar")]
    [RequestSizeLimit(104857600)] // 100 MB
    [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
    public async Task<ActionResult<UserDto>> UploadAvatar([FromForm] IFormFile file, [FromServices] IWebHostEnvironment env)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        // Fallback to ContentRootPath/wwwroot if WebRootPath is null
        var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        var uploadsFolder = Path.Combine(webRoot, "avatars");
        
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var avatarUrl = $"/avatars/{uniqueFileName}";
        return await Mediator.Send(new UpdateAvatarUrlCommand(avatarUrl));
    }
}
