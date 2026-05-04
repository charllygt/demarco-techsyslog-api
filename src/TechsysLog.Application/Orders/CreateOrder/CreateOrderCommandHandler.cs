using TechsysLog.Application.Abstractions.DomainServices;
using TechsysLog.Application.Abstractions.Events;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Domain.Common;
using TechsysLog.Domain.Orders;
using TechsysLog.Domain.Orders.ValueObjects;

namespace TechsysLog.Application.Orders.CreateOrder;

internal sealed class CreateOrderCommandHandler(
    IOrderRepository orders,
    IOrderNumberGenerator numberGenerator,
    IDomainEventDispatcher events,
    IDateTimeProvider clock) : ICommandHandler<CreateOrderCommand, CreateOrderResponse>
{
    public async Task<Result<CreateOrderResponse>> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        var cepResult = Cep.Create(command.ShippingAddress.Cep);
        if (cepResult.IsFailure) return Result.Failure<CreateOrderResponse>(cepResult.Error);

        var addressResult = Address.Create(
            cepResult.Value,
            command.ShippingAddress.Street,
            command.ShippingAddress.Number,
            command.ShippingAddress.Neighborhood,
            command.ShippingAddress.City,
            command.ShippingAddress.State);
        if (addressResult.IsFailure) return Result.Failure<CreateOrderResponse>(addressResult.Error);

        var moneyResult = Money.Create(command.Value);
        if (moneyResult.IsFailure) return Result.Failure<CreateOrderResponse>(moneyResult.Error);

        var number = await numberGenerator.NextAsync(ct).ConfigureAwait(false);

        var orderResult = Order.Create(
            number, command.Description, moneyResult.Value, addressResult.Value, command.CreatedBy, clock);
        if (orderResult.IsFailure) return Result.Failure<CreateOrderResponse>(orderResult.Error);

        var order = orderResult.Value;
        await orders.AddAsync(order, ct).ConfigureAwait(false);
        await events.DispatchAsync(order.DomainEvents, ct).ConfigureAwait(false);
        order.ClearDomainEvents();

        return Result.Success(new CreateOrderResponse(order.Id.Value, order.Number.Value));
    }
}
