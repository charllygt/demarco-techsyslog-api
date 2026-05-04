using TechsysLog.Domain.Common;
using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Orders.ValueObjects;

namespace TechsysLog.Domain.Orders.Events;

public sealed record OrderCreatedEvent(
    OrderId OrderId,
    OrderNumber Number,
    UserId CreatedBy,
    DateTime OccurredOn) : IDomainEvent;
