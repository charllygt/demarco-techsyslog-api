using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Domain.Common.Ids;

namespace TechsysLog.Application.Orders.RegisterDelivery;

public sealed record RegisterDeliveryCommand(OrderId OrderId, DateTime DeliveredAtUtc) : ICommand;
