using MongoDB.Driver;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Orders;

namespace TechsysLog.Infrastructure.Persistence.Mongo.Repositories;

internal sealed class OrderRepository(MongoContext context) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(OrderId id, CancellationToken ct) =>
        await context.Orders.Find(o => o.Id == id).FirstOrDefaultAsync(ct).ConfigureAwait(false);

    public Task AddAsync(Order order, CancellationToken ct) =>
        context.Orders.InsertOneAsync(order, cancellationToken: ct);

    public Task UpdateAsync(Order order, CancellationToken ct) =>
        context.Orders.ReplaceOneAsync(o => o.Id == order.Id, order, cancellationToken: ct);

    public async Task<IReadOnlyList<Order>> ListAsync(int skip, int take, CancellationToken ct)
    {
        var orders = await context.Orders
            .Find(FilterDefinition<Order>.Empty)
            .SortByDescending(o => o.CreatedAt)
            .Skip(skip).Limit(take)
            .ToListAsync(ct)
            .ConfigureAwait(false);
        return orders;
    }
}
