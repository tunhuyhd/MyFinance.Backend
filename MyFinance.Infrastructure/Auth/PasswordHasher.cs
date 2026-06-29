using MyFinance.Application.Common.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace MyFinance.Infrastructure.Auth;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BC.HashPassword(password, workFactor: 11);
    public bool Verify(string password, string hash) => BC.Verify(password, hash);
}
