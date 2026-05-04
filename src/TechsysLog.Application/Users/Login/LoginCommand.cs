using TechsysLog.Application.Abstractions.Messaging;

namespace TechsysLog.Application.Users.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;
