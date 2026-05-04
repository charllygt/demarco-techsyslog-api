using MongoDB.Bson.Serialization;
using TechsysLog.Domain.Notifications;

namespace TechsysLog.Infrastructure.Persistence.Mongo.Conventions;

internal static class NotificationClassMap
{
    public static void Register()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Notification)))
        {
            BsonClassMap.RegisterClassMap<Notification>(cm =>
            {
                cm.AutoMap();
                cm.IdMemberMap?.SetSerializer(new NotificationIdSerializer());
                // DomainEvents é herdado de AggregateRoot<NotificationId> — não persistir
                var domainEventsMap = cm.GetMemberMap(nameof(Notification.DomainEvents));
                if (domainEventsMap is not null)
                    cm.UnmapMember(domainEventsMap.MemberInfo);
                cm.SetIgnoreExtraElements(true);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(NotificationRecipient)))
        {
            BsonClassMap.RegisterClassMap<NotificationRecipient>(cm =>
            {
                // NotificationRecipient tem props readonly — mapeamento explícito + MapCreator.
                cm.MapMember(r => r.UserId).SetSerializer(new UserIdSerializer());
                cm.MapMember(r => r.ReadAt);
                cm.MapCreator(r => new NotificationRecipient(r.UserId, r.ReadAt));
            });
        }
    }
}
