using TechsysLog.Domain.Common;
using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Notifications.Enums;

namespace TechsysLog.Domain.Notifications;

public sealed class Notification : AggregateRoot<NotificationId>
{
    public NotificationType Type { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // List<> com private setter — necessário para Mongo deserializar via convenção idiomática.
    // Mutações controladas pelos métodos do agregado; consumidores recebem como IReadOnlyList.
    public List<NotificationRecipient> Recipients { get; private set; } = [];

    private Notification(
        NotificationId id,
        NotificationType type,
        string title,
        string message,
        DateTime createdAt) : base(id)
    {
        Type = type;
        Title = title;
        Message = message;
        CreatedAt = createdAt;
    }

    public static Result<Notification> CreateForUsers(
        NotificationType type,
        string title,
        string message,
        IEnumerable<UserId> recipients,
        IDateTimeProvider clock)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Notification>(NotificationErrors.TitleRequired);

        if (string.IsNullOrWhiteSpace(message))
            return Result.Failure<Notification>(NotificationErrors.MessageRequired);

        var recipientsList = recipients?.ToList() ?? [];
        if (recipientsList.Count == 0)
            return Result.Failure<Notification>(NotificationErrors.RecipientsRequired);

        var notification = new Notification(NotificationId.New(), type, title.Trim(), message.Trim(), clock.UtcNow);
        foreach (var userId in recipientsList)
        {
            notification.Recipients.Add(new NotificationRecipient(userId));
        }

        return Result.Success(notification);
    }

    public Result MarkAsReadBy(UserId userId, DateTime now)
    {
        var recipient = Recipients.FirstOrDefault(r => r.UserId == userId);
        if (recipient is null)
            return Result.Failure(NotificationErrors.NotARecipient);

        recipient.MarkAsRead(now);
        return Result.Success();
    }
}
