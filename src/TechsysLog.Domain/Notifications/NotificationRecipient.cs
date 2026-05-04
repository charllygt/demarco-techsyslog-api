using TechsysLog.Domain.Common.Ids;

namespace TechsysLog.Domain.Notifications;

public sealed class NotificationRecipient
{
    public UserId UserId { get; }
    public DateTime? ReadAt { get; private set; }

    internal NotificationRecipient(UserId userId, DateTime? readAt = null)
    {
        UserId = userId;
        ReadAt = readAt;
    }

    internal void MarkAsRead(DateTime now)
    {
        ReadAt ??= now;   // idempotente — primeira leitura "vence"
    }
}
