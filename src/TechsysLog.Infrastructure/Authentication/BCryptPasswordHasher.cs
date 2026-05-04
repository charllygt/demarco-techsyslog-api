using TechsysLog.Application.Abstractions.Authentication;

namespace TechsysLog.Infrastructure.Authentication;

internal sealed class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string plainTextPassword) =>
        BCrypt.Net.BCrypt.HashPassword(plainTextPassword, WorkFactor);

    public bool Verify(string plainTextPassword, string hashedPassword) =>
        BCrypt.Net.BCrypt.Verify(plainTextPassword, hashedPassword);
}
