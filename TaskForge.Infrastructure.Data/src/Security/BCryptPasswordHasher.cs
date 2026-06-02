using TaskForge.Domain.Services;

namespace TaskForge.Infrastructure.Data.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    // Cost factor regulates compilation processing overhead. 12 is a standard balance for 2026 systems.
    private const int WorkFactor = 12;

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
    }
}