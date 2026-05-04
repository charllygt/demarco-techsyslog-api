using MongoDB.Driver;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Notifications;

namespace TechsysLog.Infrastructure.Persistence.Mongo.Repositories;

internal sealed class NotificationRepository(MongoContext context) : INotificationRepository
{
    public async Task<Notification?> GetByIdAsync(NotificationId id, CancellationToken ct) =>
        await context.Notifications.Find(n => n.Id == id).FirstOrDefaultAsync(ct).ConfigureAwait(false);

    public Task AddAsync(Notification notification, CancellationToken ct) =>
        context.Notifications.InsertOneAsync(notification, cancellationToken: ct);

    public Task UpdateAsync(Notification notification, CancellationToken ct) =>
        context.Notifications.ReplaceOneAsync(n => n.Id == notification.Id, notification, cancellationToken: ct);

    public async Task<IReadOnlyList<Notification>> ListForUserAsync(UserId userId, int skip, int take, CancellationToken ct)
    {
        // Recipients é IReadOnlyCollection (computed) — LINQ provider Mongo não traduz.
        // Usar BsonDocument filter direto pelo nome do field persistido (recipients.userId).
        var elementFilter = Builders<NotificationRecipient>.Filter.Eq(r => r.UserId, userId);
        var filter = Builders<Notification>.Filter.ElemMatch("recipients", elementFilter);

        var notifications = await context.Notifications
            .Find(filter)
            .SortByDescending(n => n.CreatedAt)
            .Skip(skip).Limit(take)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return notifications;
    }
}
