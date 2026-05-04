using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Orders.Common;
using TechsysLog.Domain.Common.Ids;

namespace TechsysLog.Application.Orders.GetOrderById;

public sealed record GetOrderByIdQuery(OrderId OrderId) : IQuery<OrderResponse>;
