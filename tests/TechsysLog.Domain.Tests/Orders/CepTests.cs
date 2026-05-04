using Shouldly;
using TechsysLog.Domain.Orders.ValueObjects;

namespace TechsysLog.Domain.Tests.Orders;

public sealed class CepTests
{
    [Theory]
    [InlineData("01310100", "01310100")]
    [InlineData("01310-100", "01310100")]
    [InlineData("  01310-100  ", "01310100")]
    public void Create_WithValidCep_ShouldNormalizeToDigitsOnly(string input, string expected)
    {
        var result = Cep.Create(input);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_Empty_ShouldFail(string? input)
    {
        var result = Cep.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CepErrors.Empty);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789")]
    [InlineData("abcdefgh")]
    [InlineData("0131O100")]
    public void Create_WithInvalidFormat_ShouldFail(string input)
    {
        var result = Cep.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CepErrors.InvalidFormat);
    }

    [Fact]
    public void Formatted_ShouldReturnWithHyphen()
    {
        var cep = Cep.Create("01310100").Value;

        cep.Formatted.ShouldBe("01310-100");
    }
}
