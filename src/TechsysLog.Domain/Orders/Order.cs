using TechsysLog.Domain.Common;
using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Orders.Enums;
using TechsysLog.Domain.Orders.Events;
using TechsysLog.Domain.Orders.ValueObjects;

namespace TechsysLog.Domain.Orders;

public sealed class Order : AggregateRoot<OrderId>
{
    public OrderNumber Number { get; private set; }
    public string Description { get; private set; }
    public Money Value { get; private set; }
    public Address ShippingAddress { get; private set; }
    public OrderStatus Status { get; private set; }
    public Delivery? Delivery { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserId CreatedBy { get; private set; }

    private Order(
        OrderId id,
        OrderNumber number,
        string description,
        Money value,
        Address shippingAddress,
        UserId createdBy,
        DateTime createdAt) : base(id)
    {
        Number = number;
        Description = description;
        Value = value;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        Delivery = null;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    public static Result<Order> Create(
        OrderNumber number,
        string description,
        Money value,
        Address shippingAddress,
        UserId createdBy,
        IDateTimeProvider clock)
    {
        if (number is null)
            return Result.Failure<Order>(OrderErrors.NumberRequired);

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<Order>(OrderErrors.DescriptionRequired);

        if (value is null)
            return Result.Failure<Order>(OrderErrors.ValueRequired);

        if (shippingAddress is null)
            return Result.Failure<Order>(OrderErrors.AddressRequired);

        if (createdBy is null)
            return Result.Failure<Order>(OrderErrors.CreatorRequired);

        var createdAt = clock.UtcNow;
        var order = new Order(OrderId.New(), number, description.Trim(), value, shippingAddress, createdBy, createdAt);

        order.Raise(new OrderCreatedEvent(order.Id, order.Number, order.CreatedBy, createdAt));

        return Result.Success(order);
    }

    public Result RegisterDelivery(DateTime deliveredAt)
    {
        if (Status == OrderStatus.Delivered)
            return Result.Failure(OrderErrors.AlreadyDelivered);

        if (deliveredAt < CreatedAt)
            return Result.Failure(OrderErrors.DeliveryDateBeforeCreation);

        var deliveryResult = Delivery.Create(deliveredAt);
        if (deliveryResult.IsFailure)
            return Result.Failure(deliveryResult.Error);

        Delivery = deliveryResult.Value;
        Status = OrderStatus.Delivered;

        Raise(new DeliveryRegisteredEvent(Id, Number, CreatedBy, deliveredAt, deliveredAt));

        return Result.Success();
    }
}
