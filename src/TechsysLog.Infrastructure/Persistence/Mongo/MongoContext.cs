using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TechsysLog.Domain.Notifications;
using TechsysLog.Domain.Orders;
using TechsysLog.Domain.Users;
using TechsysLog.Infrastructure.Persistence.Mongo.Conventions;

namespace TechsysLog.Infrastructure.Persistence.Mongo;

internal sealed class MongoContext
{
    public IMongoDatabase Database { get; }

    public MongoContext(IOptions<MongoSettings> options, IMongoClient client)
    {
        MongoConventionsRegistrar.RegisterAll();
        Database = client.GetDatabase(options.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => Database.GetCollection<User>("users");
    public IMongoCollection<Order> Orders => Database.GetCollection<Order>("orders");
    public IMongoCollection<Notification> Notifications => Database.GetCollection<Notification>("notifications");
    public IMongoCollection<MongoOrderCounter> OrderCounters => Database.GetCollection<MongoOrderCounter>("counters");
}

internal sealed class MongoOrderCounter
{
    public string Id { get; set; } = string.Empty;
    public int Seq { get; set; }
}
