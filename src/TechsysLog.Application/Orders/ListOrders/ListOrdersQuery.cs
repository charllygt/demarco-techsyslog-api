using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Orders.Common;

namespace TechsysLog.Application.Orders.ListOrders;

public sealed record ListOrdersQuery(int Skip = 0, int Take = 20) : IQuery<IReadOnlyList<OrderResponse>>;
