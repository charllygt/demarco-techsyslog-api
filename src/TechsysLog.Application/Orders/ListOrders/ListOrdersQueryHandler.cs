using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Application.Orders.Common;
using TechsysLog.Application.Orders.GetOrderById;
using TechsysLog.Domain.Common;

namespace TechsysLog.Application.Orders.ListOrders;

internal sealed class ListOrdersQueryHandler(IOrderRepository orders)
    : IQueryHandler<ListOrdersQuery, IReadOnlyList<OrderResponse>>
{
    public async Task<Result<IReadOnlyList<OrderResponse>>> Handle(ListOrdersQuery query, CancellationToken ct)
    {
        var list = await orders.ListAsync(query.Skip, query.Take, ct).ConfigureAwait(false);
        IReadOnlyList<OrderResponse> response = list.Select(OrderMapping.ToResponse).ToList();
        return Result.Success(response);
    }
}
