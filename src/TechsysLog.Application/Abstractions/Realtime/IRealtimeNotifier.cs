using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Notifications.Enums;

namespace TechsysLog.Application.Abstractions.Realtime;

public interface IRealtimeNotifier
{
    Task NotifyAsync(RealtimeNotificationPayload payload, CancellationToken ct);
}

public sealed record RealtimeNotificationPayload(
    NotificationId NotificationId,
    NotificationType Type,
    string Title,
    string Message,
    DateTime CreatedAt,
    IReadOnlyList<UserId> Recipients);
