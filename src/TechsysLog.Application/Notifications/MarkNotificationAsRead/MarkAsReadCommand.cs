using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Domain.Common.Ids;

namespace TechsysLog.Application.Notifications.MarkNotificationAsRead;

public sealed record MarkAsReadCommand(NotificationId NotificationId, UserId UserId) : ICommand;
