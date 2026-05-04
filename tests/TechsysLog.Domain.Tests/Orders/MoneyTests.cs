using Shouldly;
using TechsysLog.Domain.Orders.ValueObjects;

namespace TechsysLog.Domain.Tests.Orders;

public sealed class MoneyTests
{
    [Fact]
    public void Create_WithPositiveAmountAndDefaultCurrency_ShouldReturnBRL()
    {
        var result = Money.Create(100.50m);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Amount.ShouldBe(100.50m);
        result.Value.Currency.ShouldBe("BRL");
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldSucceed()
    {
        var result = Money.Create(0m);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldFail()
    {
        var result = Money.Create(-1m);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(MoneyErrors.NegativeAmount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyCurrency_ShouldFail(string? currency)
    {
        var result = Money.Create(10m, currency);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(MoneyErrors.InvalidCurrency);
    }

    [Theory]
    [InlineData("BR")]
    [InlineData("REAL")]
    public void Create_WithCurrencyCodeNotThreeChars_ShouldFail(string currency)
    {
        var result = Money.Create(10m, currency);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(MoneyErrors.InvalidCurrency);
    }

    [Fact]
    public void Equality_SameAmountAndCurrency_ShouldBeEqual()
    {
        var a = Money.Create(10m).Value;
        var b = Money.Create(10m).Value;

        a.ShouldBe(b);
    }
}
