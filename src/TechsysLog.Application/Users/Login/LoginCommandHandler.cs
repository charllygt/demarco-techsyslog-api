using TechsysLog.Application.Abstractions.Authentication;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Domain.Common;
using TechsysLog.Domain.Users;
using TechsysLog.Domain.Users.ValueObjects;

namespace TechsysLog.Application.Users.Login;

internal sealed class LoginCommandHandler(
    IUserRepository users,
    IPasswordHasher hasher,
    ITokenService tokens) : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken ct)
    {
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
            return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);

        var user = await users.GetByEmailAsync(emailResult.Value, ct).ConfigureAwait(false);
        if (user is null || !hasher.Verify(command.Password, user.PasswordHash.Value))
            return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);

        var token = tokens.GenerateToken(user);
        return Result.Success(new LoginResponse(
            token.AccessToken, token.ExpiresAtUtc, user.Id.Value, user.Name, user.Email.Value));
    }
}
