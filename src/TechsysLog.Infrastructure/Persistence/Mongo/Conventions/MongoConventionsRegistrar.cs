using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using TechsysLog.Domain.Common.Ids;

namespace TechsysLog.Infrastructure.Persistence.Mongo.Conventions;

internal static class MongoConventionsRegistrar
{
    private static int _registered;

    public static void RegisterAll()
    {
        if (Interlocked.Exchange(ref _registered, 1) == 1) return;

        // Conventions globais
        var conventions = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
            new EnumRepresentationConvention(BsonType.String)
        };
        ConventionRegistry.Register("TechsysLogConventions", conventions, _ => true);

        // MongoDB.Driver 3.x removeu GuidRepresentation default. Registrar explicitamente
        // para evitar "GuidSerializer cannot serialize a Guid when GuidRepresentation is Unspecified".
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        // Registro global dos StronglyTypedIds — garante que o serializer custom é usado
        // mesmo quando a propriedade é herdada (Entity<TId>.Id).
        BsonSerializer.TryRegisterSerializer(typeof(UserId), new UserIdSerializer());
        BsonSerializer.TryRegisterSerializer(typeof(OrderId), new OrderIdSerializer());
        BsonSerializer.TryRegisterSerializer(typeof(NotificationId), new NotificationIdSerializer());

        UserClassMap.Register();
        OrderClassMap.Register();
        NotificationClassMap.Register();
    }
}
