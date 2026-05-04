using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Orders;

namespace TechsysLog.Application.Abstractions.Persistence;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(OrderId id, CancellationToken ct);
    Task AddAsync(Order order, CancellationToken ct);
    Task UpdateAsync(Order order, CancellationToken ct);
    Task<IReadOnlyList<Order>> ListAsync(int skip, int take, CancellationToken ct);
}
