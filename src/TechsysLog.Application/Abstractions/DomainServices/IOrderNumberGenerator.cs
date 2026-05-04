using TechsysLog.Domain.Orders.ValueObjects;

namespace TechsysLog.Application.Abstractions.DomainServices;

public interface IOrderNumberGenerator
{
    Task<OrderNumber> NextAsync(CancellationToken ct);
}
