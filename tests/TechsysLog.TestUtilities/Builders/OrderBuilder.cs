using TechsysLog.Domain.Common;
using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Orders;
using TechsysLog.Domain.Orders.ValueObjects;
using TechsysLog.TestUtilities.Doubles;

namespace TechsysLog.TestUtilities.Builders;

public sealed class OrderBuilder
{
    private OrderNumber _number = OrderNumber.Create("ORD-202604-000001").Value;
    private string _description = "Item de teste";
    private Money _value = Money.Create(100m).Value;
    private Address _address = AddressBuilder.New().Build();
    private UserId _createdBy = UserId.New();
    private IDateTimeProvider _clock = FixedDateTimeProvider.AtUtc(2026, 4, 30);

    public static OrderBuilder New() => new();

    public OrderBuilder WithNumber(string number)
    { _number = OrderNumber.Create(number).Value; return this; }
    public OrderBuilder WithDescription(string desc) { _description = desc; return this; }
    public OrderBuilder WithValue(decimal amount)
    { _value = Money.Create(amount).Value; return this; }
    public OrderBuilder WithAddress(Address address) { _address = address; return this; }
    public OrderBuilder OwnedBy(UserId id) { _createdBy = id; return this; }
    public OrderBuilder At(DateTime utc)
    { _clock = new FixedDateTimeProvider(utc); return this; }

    public Order Build() =>
        Order.Create(_number, _description, _value, _address, _createdBy, _clock).Value;
}
