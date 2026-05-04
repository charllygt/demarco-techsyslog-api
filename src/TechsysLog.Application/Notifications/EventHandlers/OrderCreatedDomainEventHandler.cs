using System.Globalization;
using TechsysLog.Application.Abstractions.Events;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Application.Abstractions.Realtime;
using TechsysLog.Domain.Common;
using TechsysLog.Domain.Notifications;
using TechsysLog.Domain.Notifications.Enums;
using TechsysLog.Domain.Orders.Events;

namespace TechsysLog.Application.Notifications.EventHandlers;

// DECISÃO: Broadcast para todos os usuários cadastrados.
// Cenário ideal em produção (documentado em ARCHITECTURE.md):
// 1. Notificar dono do pedido (Order.CreatedBy) sobre SEU pedido.
// 2. Role "Operator/Logistics" recebe TODAS as movimentações operacionais.
// 3. Suportar opt-in/out por tipo de notificação.
// A abstração Notification.CreateForUsers já está pronta para essa evolução —
// basta trocar a query que resolve a lista de destinatários.
internal sealed class OrderCreatedDomainEventHandler(
    INotificationRepository notifications,
    IUserRepository users,
    IRealtimeNotifier realtime,
    IDateTimeProvider clock) : IDomainEventHandler<OrderCreatedEvent>
{
    public async Task HandleAsync(OrderCreatedEvent domainEvent, CancellationToken ct)
    {
        var allUserIds = await users.GetAllUserIdsAsync(ct).ConfigureAwait(false);
        if (allUserIds.Count == 0) return;

        var title = $"Novo pedido {domainEvent.Number.Value}";
        var message = string.Create(CultureInfo.InvariantCulture,
            $"O pedido {domainEvent.Number.Value} foi criado em {domainEvent.OccurredOn:yyyy-MM-dd HH:mm} UTC.");

        var notificationResult = Notification.CreateForUsers(
            NotificationType.OrderCreated, title, message, allUserIds, clock);
        if (notificationResult.IsFailure) return;

        var notification = notificationResult.Value;
        await notifications.AddAsync(notification, ct).ConfigureAwait(false);

        var payload = new RealtimeNotificationPayload(
            notification.Id, notification.Type, notification.Title, notification.Message,
            notification.CreatedAt, allUserIds);
        await realtime.NotifyAsync(payload, ct).ConfigureAwait(false);
    }
}
