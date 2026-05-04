namespace TechsysLog.Application.Orders.CreateOrder;

public sealed record AddressDto(
    string Cep, string Street, string Number, string Neighborhood, string City, string State);
