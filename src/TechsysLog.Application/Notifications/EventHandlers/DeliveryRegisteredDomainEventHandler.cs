using System.Globalization;
using TechsysLog.Application.Abstractions.Events;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Application.Abstractions.Realtime;
using TechsysLog.Domain.Common;
using TechsysLog.Domain.Notifications;
using TechsysLog.Domain.Notifications.Enums;
using TechsysLog.Domain.Orders.Events;

namespace TechsysLog.Application.Notifications.EventHandlers;

internal sealed class DeliveryRegisteredDomainEventHandler(
    INotificationRepository notifications,
    IUserRepository users,
    IRealtimeNotifier realtime,
    IDateTimeProvider clock) : IDomainEventHandler<DeliveryRegisteredEvent>
{
    public async Task HandleAsync(DeliveryRegisteredEvent domainEvent, CancellationToken ct)
    {
        var allUserIds = await users.GetAllUserIdsAsync(ct).ConfigureAwait(false);
        if (allUserIds.Count == 0) return;

        var title = $"Entrega registrada para {domainEvent.Number.Value}";
        var message = string.Create(CultureInfo.InvariantCulture,
            $"O pedido {domainEvent.Number.Value} foi entregue em {domainEvent.DeliveredAt:yyyy-MM-dd HH:mm} UTC.");

        var notificationResult = Notification.CreateForUsers(
            NotificationType.DeliveryRegistered, title, message, allUserIds, clock);
        if (notificationResult.IsFailure) return;

        var notification = notificationResult.Value;
        await notifications.AddAsync(notification, ct).ConfigureAwait(false);

        var payload = new RealtimeNotificationPayload(
            notification.Id, notification.Type, notification.Title, notification.Message,
            notification.CreatedAt, allUserIds);
        await realtime.NotifyAsync(payload, ct).ConfigureAwait(false);
    }
}
