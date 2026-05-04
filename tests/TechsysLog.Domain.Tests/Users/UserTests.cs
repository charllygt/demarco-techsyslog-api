using Shouldly;
using TechsysLog.Domain.Users;
using TechsysLog.Domain.Users.ValueObjects;

namespace TechsysLog.Domain.Tests.Users;

public sealed class UserTests
{
    private static readonly Email ValidEmail = Email.Create("user@test.com").Value;
    private static readonly PasswordHash ValidHash = PasswordHash.Create("$2a$12$abcdefghijklmnop").Value;

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = User.Create("João Silva", ValidEmail, ValidHash);

        result.IsSuccess.ShouldBeTrue();
        var user = result.Value;
        user.Id.Value.ShouldNotBe(Guid.Empty);
        user.Name.ShouldBe("João Silva");
        user.Email.ShouldBe(ValidEmail);
        user.PasswordHash.ShouldBe(ValidHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ShouldFail(string? name)
    {
        var result = User.Create(name!, ValidEmail, ValidHash);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.NameRequired);
    }

    [Fact]
    public void Create_WithNameAbove150Chars_ShouldFail()
    {
        var name = new string('a', 151);

        var result = User.Create(name, ValidEmail, ValidHash);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.NameTooLong);
    }

    [Fact]
    public void Create_WithNullEmail_ShouldFail()
    {
        var result = User.Create("João", null!, ValidHash);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.EmailRequired);
    }

    [Fact]
    public void Create_WithNullPasswordHash_ShouldFail()
    {
        var result = User.Create("João", ValidEmail, null!);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.PasswordHashRequired);
    }

    [Fact]
    public void ChangePassword_WithValidHash_ShouldUpdate()
    {
        var user = User.Create("João", ValidEmail, ValidHash).Value;
        var newHash = PasswordHash.Create("$2a$12$newhash").Value;

        var result = user.ChangePassword(newHash);

        result.IsSuccess.ShouldBeTrue();
        user.PasswordHash.ShouldBe(newHash);
    }

    [Fact]
    public void ChangePassword_WithNull_ShouldFail()
    {
        var user = User.Create("João", ValidEmail, ValidHash).Value;

        var result = user.ChangePassword(null!);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.PasswordHashRequired);
        user.PasswordHash.ShouldBe(ValidHash);
    }
}
