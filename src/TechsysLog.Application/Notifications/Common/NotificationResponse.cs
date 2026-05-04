namespace TechsysLog.Application.Notifications.Common;

public sealed record NotificationResponse(
    Guid Id, string Type, string Title, string Message, DateTime CreatedAt, DateTime? ReadAt);
