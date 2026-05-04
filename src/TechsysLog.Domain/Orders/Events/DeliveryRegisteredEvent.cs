using TechsysLog.Domain.Common;
using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Orders.ValueObjects;

namespace TechsysLog.Domain.Orders.Events;

public sealed record DeliveryRegisteredEvent(
    OrderId OrderId,
    OrderNumber Number,
    UserId CreatedBy,
    DateTime DeliveredAt,
    DateTime OccurredOn) : IDomainEvent;
