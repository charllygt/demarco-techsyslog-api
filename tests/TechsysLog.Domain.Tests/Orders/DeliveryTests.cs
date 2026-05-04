using Shouldly;
using TechsysLog.Domain.Orders.ValueObjects;

namespace TechsysLog.Domain.Tests.Orders;

public sealed class DeliveryTests
{
    [Fact]
    public void Create_WithValidDate_ShouldSucceed()
    {
        var date = new DateTime(2026, 4, 30, 14, 0, 0, DateTimeKind.Utc);

        var result = Delivery.Create(date);

        result.IsSuccess.ShouldBeTrue();
        result.Value.DeliveredAt.ShouldBe(date);
    }

    [Fact]
    public void Create_WithNonUtcDate_ShouldFail()
    {
        var date = new DateTime(2026, 4, 30, 14, 0, 0, DateTimeKind.Local);

        var result = Delivery.Create(date);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(DeliveryErrors.DateMustBeUtc);
    }

    [Fact]
    public void Equality_SameDate_ShouldBeEqual()
    {
        var date = new DateTime(2026, 4, 30, 14, 0, 0, DateTimeKind.Utc);
        var a = Delivery.Create(date).Value;
        var b = Delivery.Create(date).Value;

        a.ShouldBe(b);
    }
}
