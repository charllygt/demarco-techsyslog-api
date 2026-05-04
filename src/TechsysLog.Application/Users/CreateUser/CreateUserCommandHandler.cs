using TechsysLog.Application.Abstractions.Authentication;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Domain.Common;
using TechsysLog.Domain.Users;
using TechsysLog.Domain.Users.ValueObjects;

namespace TechsysLog.Application.Users.CreateUser;

internal sealed class CreateUserCommandHandler(
    IUserRepository users,
    IPasswordHasher hasher) : ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
            return Result.Failure<CreateUserResponse>(emailResult.Error);

        if (await users.EmailExistsAsync(emailResult.Value, ct).ConfigureAwait(false))
            return Result.Failure<CreateUserResponse>(UserErrors.EmailAlreadyInUse);

        var hashed = hasher.Hash(command.Password);
        var hashResult = PasswordHash.Create(hashed);
        if (hashResult.IsFailure)
            return Result.Failure<CreateUserResponse>(hashResult.Error);

        var userResult = User.Create(command.Name, emailResult.Value, hashResult.Value);
        if (userResult.IsFailure)
            return Result.Failure<CreateUserResponse>(userResult.Error);

        var user = userResult.Value;
        await users.AddAsync(user, ct).ConfigureAwait(false);

        return Result.Success(new CreateUserResponse(user.Id.Value, user.Name, user.Email.Value));
    }
}
