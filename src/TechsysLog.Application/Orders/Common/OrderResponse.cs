namespace TechsysLog.Application.Orders.Common;

public sealed record OrderResponse(
    Guid Id,
    string Number,
    string Description,
    decimal Value,
    string Currency,
    AddressView ShippingAddress,
    string Status,
    DateTime CreatedAt,
    DateTime? DeliveredAt);

public sealed record AddressView(
    string Cep, string Street, string Number, string Neighborhood, string City, string State);
