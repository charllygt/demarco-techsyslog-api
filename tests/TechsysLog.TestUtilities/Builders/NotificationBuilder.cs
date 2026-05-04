using TechsysLog.Domain.Common;
using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Notifications;
using TechsysLog.Domain.Notifications.Enums;
using TechsysLog.TestUtilities.Doubles;

namespace TechsysLog.TestUtilities.Builders;

public sealed class NotificationBuilder
{
    private NotificationType _type = NotificationType.OrderCreated;
    private string _title = "Notificação de teste";
    private string _message = "Mensagem de teste";
    private List<UserId> _recipients = [UserId.New()];
    private IDateTimeProvider _clock = FixedDateTimeProvider.AtUtc(2026, 4, 30);

    public static NotificationBuilder New() => new();

    public NotificationBuilder OfType(NotificationType type) { _type = type; return this; }
    public NotificationBuilder WithTitle(string title) { _title = title; return this; }
    public NotificationBuilder WithMessage(string msg) { _message = msg; return this; }
    public NotificationBuilder ForRecipients(params UserId[] recipients)
    { _recipients = [.. recipients]; return this; }
    public NotificationBuilder At(DateTime utc) { _clock = new FixedDateTimeProvider(utc); return this; }

    public Notification Build() =>
        Notification.CreateForUsers(_type, _title, _message, _recipients, _clock).Value;
}
