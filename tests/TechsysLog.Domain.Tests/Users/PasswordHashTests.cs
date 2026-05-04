using Shouldly;
using TechsysLog.Domain.Users.ValueObjects;

namespace TechsysLog.Domain.Tests.Users;

public sealed class PasswordHashTests
{
    [Fact]
    public void Create_WithValidHash_ShouldReturnSuccess()
    {
        const string hash = "$2a$12$abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUV";

        var result = PasswordHash.Create(hash);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(hash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyOrWhitespace_ShouldFail(string? input)
    {
        var result = PasswordHash.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(PasswordHashErrors.Empty);
    }

    [Fact]
    public void ToString_ShouldNeverExposeRawHash()
    {
        const string hash = "$2a$12$abcdefghij";
        var passwordHash = PasswordHash.Create(hash).Value;

        passwordHash.ToString().ShouldNotContain(hash);
    }
}
