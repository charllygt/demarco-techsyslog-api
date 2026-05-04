using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TechsysLog.Domain.Notifications;
using TechsysLog.Domain.Orders;
using TechsysLog.Domain.Users;

namespace TechsysLog.Infrastructure.Persistence.Mongo.Indexes;

internal sealed class MongoIndexInitializer(
    IServiceScopeFactory scopeFactory,
    ILogger<MongoIndexInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken ct)
    {
        logger.LogInformation("Inicializando índices MongoDB");

        await using var scope = scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<MongoContext>();

        var userKeys = Builders<User>.IndexKeys.Ascending(u => u.Email);
        await context.Users.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(userKeys, new CreateIndexOptions { Unique = true, Name = "ix_users_email" }),
            cancellationToken: ct).ConfigureAwait(false);

        var orderNumberKeys = Builders<Order>.IndexKeys.Ascending(o => o.Number);
        await context.Orders.Indexes.CreateOneAsync(
            new CreateIndexModel<Order>(orderNumberKeys, new CreateIndexOptions { Unique = true, Name = "ix_orders_number" }),
            cancellationToken: ct).ConfigureAwait(false);

        var orderCreatedByKeys = Builders<Order>.IndexKeys.Ascending(o => o.CreatedBy);
        await context.Orders.Indexes.CreateOneAsync(
            new CreateIndexModel<Order>(orderCreatedByKeys, new CreateIndexOptions { Name = "ix_orders_createdBy" }),
            cancellationToken: ct).ConfigureAwait(false);

        var notificationRecipientKeys = Builders<Notification>.IndexKeys
            .Ascending("recipients.UserId");
        await context.Notifications.Indexes.CreateOneAsync(
            new CreateIndexModel<Notification>(notificationRecipientKeys, new CreateIndexOptions { Name = "ix_notifications_recipients_userId" }),
            cancellationToken: ct).ConfigureAwait(false);

        logger.LogInformation("Índices MongoDB OK");
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
