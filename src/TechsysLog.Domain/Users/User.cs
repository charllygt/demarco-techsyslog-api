using TechsysLog.Domain.Common;
using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Users.ValueObjects;

namespace TechsysLog.Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    private const int MaxNameLength = 150;

    public string Name { get; private set; }
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }

    private User(UserId id, string name, Email email, PasswordHash passwordHash) : base(id)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
    }

    public static Result<User> Create(string name, Email email, PasswordHash passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<User>(UserErrors.NameRequired);

        if (name.Trim().Length > MaxNameLength)
            return Result.Failure<User>(UserErrors.NameTooLong);

        if (email is null)
            return Result.Failure<User>(UserErrors.EmailRequired);

        if (passwordHash is null)
            return Result.Failure<User>(UserErrors.PasswordHashRequired);

        return Result.Success(new User(UserId.New(), name.Trim(), email, passwordHash));
    }

    public Result ChangePassword(PasswordHash newHash)
    {
        if (newHash is null)
            return Result.Failure(UserErrors.PasswordHashRequired);

        PasswordHash = newHash;
        return Result.Success();
    }
}
