using TechsysLog.Application.Abstractions.Messaging;

namespace TechsysLog.Application.Users.CreateUser;

public sealed record CreateUserCommand(string Name, string Email, string Password)
    : ICommand<CreateUserResponse>;
