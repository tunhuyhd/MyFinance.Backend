using MyFinance.Domain.Entities;

namespace MyFinance.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
