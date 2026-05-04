using Shouldly;
using TechsysLog.Domain.Orders.ValueObjects;

namespace TechsysLog.Domain.Tests.Orders;

public sealed class OrderNumberTests
{
    [Theory]
    [InlineData("ORD-202604-000001")]
    [InlineData("ORD-202612-999999")]
    [InlineData("ORD-202401-000123")]
    public void Create_WithValidFormat_ShouldSucceed(string input)
    {
        var result = OrderNumber.Create(input);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(input);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_Empty_ShouldFail(string? input)
    {
        var result = OrderNumber.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(OrderNumberErrors.Empty);
    }

    [Theory]
    [InlineData("ORD-202604-1")]
    [InlineData("ORDER-202604-000001")]
    [InlineData("ORD-2026-000001")]
    [InlineData("ORD-20260413-000001")]
    [InlineData("ord-202604-000001")]
    [InlineData("ORD202604000001")]
    public void Create_WithInvalidFormat_ShouldFail(string input)
    {
        var result = OrderNumber.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(OrderNumberErrors.InvalidFormat);
    }

    [Fact]
    public void Generate_ShouldProduceValidOrderNumber()
    {
        var number = OrderNumber.Generate(year: 2026, month: 4, sequential: 42);

        number.Value.ShouldBe("ORD-202604-000042");
    }
}
