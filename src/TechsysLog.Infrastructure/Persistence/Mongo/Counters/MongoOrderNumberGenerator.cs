using System.Globalization;
using MongoDB.Driver;
using TechsysLog.Application.Abstractions.DomainServices;
using TechsysLog.Domain.Common;
using TechsysLog.Domain.Orders.ValueObjects;

namespace TechsysLog.Infrastructure.Persistence.Mongo.Counters;

internal sealed class MongoOrderNumberGenerator(
    MongoContext context,
    IDateTimeProvider clock) : IOrderNumberGenerator
{
    public async Task<OrderNumber> NextAsync(CancellationToken ct)
    {
        var now = clock.UtcNow;
        var counterId = string.Create(CultureInfo.InvariantCulture, $"order:{now:yyyyMM}");

        var update = Builders<MongoOrderCounter>.Update.Inc(c => c.Seq, 1);
        var options = new FindOneAndUpdateOptions<MongoOrderCounter>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After
        };

        var counter = await context.OrderCounters
            .FindOneAndUpdateAsync<MongoOrderCounter>(c => c.Id == counterId, update, options, ct)
            .ConfigureAwait(false);

        return OrderNumber.Generate(now.Year, now.Month, counter.Seq);
    }
}
