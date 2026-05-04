using MongoDB.Bson.Serialization;
using TechsysLog.Domain.Users;

namespace TechsysLog.Infrastructure.Persistence.Mongo.Conventions;

internal static class UserClassMap
{
    public static void Register()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(User))) return;

        BsonClassMap.RegisterClassMap<User>(cm =>
        {
            cm.AutoMap();
            // Id é herdado de Entity<UserId> — AutoMap reconhece via convenção. Customizar serializer:
            cm.IdMemberMap?.SetSerializer(new UserIdSerializer());
            cm.GetMemberMap(nameof(User.Email)).SetSerializer(new EmailSerializer());
            cm.GetMemberMap(nameof(User.PasswordHash)).SetSerializer(new PasswordHashSerializer());
            // DomainEvents é herdado de AggregateRoot<UserId> — não persistir
            var domainEventsMap = cm.GetMemberMap(nameof(User.DomainEvents));
            if (domainEventsMap is not null)
                cm.UnmapMember(domainEventsMap.MemberInfo);
            cm.SetIgnoreExtraElements(true);
        });
    }
}
