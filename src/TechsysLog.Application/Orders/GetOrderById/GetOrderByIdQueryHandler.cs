using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Application.Orders.Common;
using TechsysLog.Domain.Common;
using TechsysLog.Domain.Orders;

namespace TechsysLog.Application.Orders.GetOrderById;

internal sealed class GetOrderByIdQueryHandler(IOrderRepository orders)
    : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery query, CancellationToken ct)
    {
        var order = await orders.GetByIdAsync(query.OrderId, ct).ConfigureAwait(false);
        if (order is null) return Result.Failure<OrderResponse>(OrderErrors.NotFound);

        return Result.Success(OrderMapping.ToResponse(order));
    }
}

internal static class OrderMapping
{
    public static OrderResponse ToResponse(Order o) =>
        new(
            o.Id.Value,
            o.Number.Value,
            o.Description,
            o.Value.Amount,
            o.Value.Currency,
            new AddressView(
                o.ShippingAddress.Cep.Formatted,
                o.ShippingAddress.Street,
                o.ShippingAddress.Number,
                o.ShippingAddress.Neighborhood,
                o.ShippingAddress.City,
                o.ShippingAddress.State),
            o.Status.ToString(),
            o.CreatedAt,
            o.Delivery?.DeliveredAt);
}
