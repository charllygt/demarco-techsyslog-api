using MongoDB.Bson.Serialization;
using TechsysLog.Domain.Orders;

namespace TechsysLog.Infrastructure.Persistence.Mongo.Conventions;

internal static class OrderClassMap
{
    public static void Register()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Order))) return;

        BsonClassMap.RegisterClassMap<Order>(cm =>
        {
            cm.AutoMap();
            cm.IdMemberMap?.SetSerializer(new OrderIdSerializer());
            cm.GetMemberMap(nameof(Order.Number)).SetSerializer(new OrderNumberSerializer());
            cm.GetMemberMap(nameof(Order.Value)).SetSerializer(new MoneySerializer());
            cm.GetMemberMap(nameof(Order.ShippingAddress)).SetSerializer(new AddressSerializer());
            cm.GetMemberMap(nameof(Order.Delivery)).SetSerializer(new DeliverySerializer());
            cm.GetMemberMap(nameof(Order.CreatedBy)).SetSerializer(new UserIdSerializer());
            // DomainEvents é herdado de AggregateRoot<OrderId> — não persistir
            var domainEventsMap = cm.GetMemberMap(nameof(Order.DomainEvents));
            if (domainEventsMap is not null)
                cm.UnmapMember(domainEventsMap.MemberInfo);
            cm.SetIgnoreExtraElements(true);
        });
    }
}
