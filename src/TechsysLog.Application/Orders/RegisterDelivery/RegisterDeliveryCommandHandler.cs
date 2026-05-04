using TechsysLog.Application.Abstractions.Events;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Domain.Common;
using TechsysLog.Domain.Orders;

namespace TechsysLog.Application.Orders.RegisterDelivery;

internal sealed class RegisterDeliveryCommandHandler(
    IOrderRepository orders,
    IDomainEventDispatcher events) : ICommandHandler<RegisterDeliveryCommand>
{
    public async Task<Result> Handle(RegisterDeliveryCommand command, CancellationToken ct)
    {
        var order = await orders.GetByIdAsync(command.OrderId, ct).ConfigureAwait(false);
        if (order is null) return Result.Failure(OrderErrors.NotFound);

        var utc = command.DeliveredAtUtc.Kind == DateTimeKind.Utc
            ? command.DeliveredAtUtc
            : command.DeliveredAtUtc.ToUniversalTime();

        var result = order.RegisterDelivery(utc);
        if (result.IsFailure) return result;

        await orders.UpdateAsync(order, ct).ConfigureAwait(false);
        await events.DispatchAsync(order.DomainEvents, ct).ConfigureAwait(false);
        order.ClearDomainEvents();

        return Result.Success();
    }
}
