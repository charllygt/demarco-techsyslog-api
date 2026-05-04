using Shouldly;
using TechsysLog.Domain.Users.ValueObjects;

namespace TechsysLog.Domain.Tests.Users;

public sealed class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("first.last+tag@sub.example.co.uk")]
    [InlineData("USER@EXAMPLE.COM")]
    public void Create_WithValidEmail_ShouldReturnSuccess(string input)
    {
        var result = Email.Create(input);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(input.Trim().ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyOrWhitespace_ShouldFailWithEmptyError(string? input)
    {
        var result = Email.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(EmailErrors.Empty);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("@nodomain.com")]
    [InlineData("noatsign.com")]
    [InlineData("two@@signs.com")]
    [InlineData("missing@dot")]
    public void Create_WithInvalidFormat_ShouldFailWithInvalidFormatError(string input)
    {
        var result = Email.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(EmailErrors.InvalidFormat);
    }

    [Fact]
    public void Create_WithLengthAbove320_ShouldFailWithTooLongError()
    {
        var local = new string('a', 320);
        var input = $"{local}@x.com";   // 320 + "@x.com" = 326 chars

        var result = Email.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(EmailErrors.TooLong);
    }

    [Fact]
    public void Equality_TwoEmailsWithSameValue_ShouldBeEqual()
    {
        var a = Email.Create("foo@bar.com").Value;
        var b = Email.Create("FOO@BAR.COM").Value;

        a.ShouldBe(b);
    }
}
