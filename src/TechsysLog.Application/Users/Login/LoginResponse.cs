namespace TechsysLog.Application.Users.Login;

public sealed record LoginResponse(string Token, DateTime ExpiresAtUtc, Guid UserId, string Name, string Email);
