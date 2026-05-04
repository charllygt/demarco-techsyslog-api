using TechsysLog.Domain.Users;
using TechsysLog.Domain.Users.ValueObjects;

namespace TechsysLog.TestUtilities.Builders;

public sealed class UserBuilder
{
    private string _name = "Test User";
    private string _email = "user@test.com";
    private string _passwordHash = "$2a$12$abcdefghijklmnopqrstuv";

    public static UserBuilder New() => new();

    public UserBuilder WithName(string name) { _name = name; return this; }
    public UserBuilder WithEmail(string email) { _email = email; return this; }
    public UserBuilder WithPasswordHash(string hash) { _passwordHash = hash; return this; }

    public User Build()
    {
        var email = Email.Create(_email).Value;
        var hash = PasswordHash.Create(_passwordHash).Value;
        return User.Create(_name, email, hash).Value;
    }
}
