using Shouldly;
using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Orders;
using TechsysLog.Domain.Orders.Enums;
using TechsysLog.Domain.Orders.Events;
using TechsysLog.Domain.Orders.ValueObjects;
using TechsysLog.TestUtilities.Builders;
using TechsysLog.TestUtilities.Doubles;

namespace TechsysLog.Domain.Tests.Orders;

public sealed class OrderTests
{
    private static OrderNumber Number => OrderNumber.Create("ORD-202604-000001").Value;
    private static Money Value => Money.Create(100m).Value;
    private static UserId Creator => UserId.New();
    private static FixedDateTimeProvider Clock => FixedDateTimeProvider.AtUtc(2026, 4, 30, 10, 0, 0);

    [Fact]
    public void Create_WithValidData_ShouldSucceedAndRaiseOrderCreatedEvent()
    {
        var address = AddressBuilder.New().Build();
        var clock = Clock;

        var result = Order.Create(Number, "Notebook Dell", Value, address, Creator, clock);

        result.IsSuccess.ShouldBeTrue();
        var order = result.Value;
        order.Number.ShouldBe(Number);
        order.Description.ShouldBe("Notebook Dell");
        order.Value.ShouldBe(Value);
        order.ShippingAddress.ShouldBe(address);
        order.Status.ShouldBe(OrderStatus.Pending);
        order.Delivery.ShouldBeNull();
        order.CreatedAt.ShouldBe(clock.UtcNow);
        order.DomainEvents.Count.ShouldBe(1);
        order.DomainEvents.OfType<OrderCreatedEvent>().ShouldHaveSingleItem();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyDescription_ShouldFail(string? description)
    {
        var result = Order.Create(Number, description!, Value, AddressBuilder.New().Build(), Creator, Clock);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(OrderErrors.DescriptionRequired);
    }

    [Fact]
    public void Create_WithNullNumber_ShouldFail()
    {
        var result = Order.Create(null!, "X", Value, AddressBuilder.New().Build(), Creator, Clock);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(OrderErrors.NumberRequired);
    }

    [Fact]
    public void Create_WithNullValue_ShouldFail()
    {
        var result = Order.Create(Number, "X", null!, AddressBuilder.New().Build(), Creator, Clock);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(OrderErrors.ValueRequired);
    }

    [Fact]
    public void Create_WithNullAddress_ShouldFail()
    {
        var result = Order.Create(Number, "X", Value, null!, Creator, Clock);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(OrderErrors.AddressRequired);
    }

    [Fact]
    public void Create_WithNullCreator_ShouldFail()
    {
        var result = Order.Create(Number, "X", Value, AddressBuilder.New().Build(), null!, Clock);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(OrderErrors.CreatorRequired);
    }

    [Fact]
    public void RegisterDelivery_WhenPending_ShouldSucceedAndRaiseEvent()
    {
        var clock = Clock;
        var order = Order.Create(Number, "X", Value, AddressBuilder.New().Build(), Creator, clock).Value;
        order.ClearDomainEvents();
        var deliveredAt = clock.UtcNow.AddHours(2);

        var result = order.RegisterDelivery(deliveredAt);

        result.IsSuccess.ShouldBeTrue();
        order.Status.ShouldBe(OrderStatus.Delivered);
        order.Delivery.ShouldNotBeNull();
        order.Delivery!.DeliveredAt.ShouldBe(deliveredAt);
        order.DomainEvents.OfType<DeliveryRegisteredEvent>().ShouldHaveSingleItem();
    }

    [Fact]
    public void RegisterDelivery_WhenAlreadyDelivered_ShouldFail()
    {
        var clock = Clock;
        var order = Order.Create(Number, "X", Value, AddressBuilder.New().Build(), Creator, clock).Value;
        order.RegisterDelivery(clock.UtcNow.AddHours(2));

        var result = order.RegisterDelivery(clock.UtcNow.AddHours(3));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(OrderErrors.AlreadyDelivered);
    }

    [Fact]
    public void RegisterDelivery_WithDateBeforeCreation_ShouldFail()
    {
        var clock = Clock;
        var order = Order.Create(Number, "X", Value, AddressBuilder.New().Build(), Creator, clock).Value;

        var result = order.RegisterDelivery(clock.UtcNow.AddDays(-1));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(OrderErrors.DeliveryDateBeforeCreation);
    }
}
