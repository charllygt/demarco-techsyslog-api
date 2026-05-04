using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Domain.Common.Ids;

namespace TechsysLog.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(
    string Description,
    decimal Value,
    AddressDto ShippingAddress,
    UserId CreatedBy) : ICommand<CreateOrderResponse>;
