using TechsysLog.Domain.Users;

namespace TechsysLog.Application.Abstractions.Authentication;

public interface ITokenService
{
    GeneratedToken GenerateToken(User user);
}

public sealed record GeneratedToken(string AccessToken, DateTime ExpiresAtUtc);
