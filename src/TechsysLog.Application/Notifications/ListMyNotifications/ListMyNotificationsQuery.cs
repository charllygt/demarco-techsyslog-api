using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Notifications.Common;
using TechsysLog.Domain.Common.Ids;

namespace TechsysLog.Application.Notifications.ListMyNotifications;

public sealed record ListMyNotificationsQuery(UserId UserId, int Skip = 0, int Take = 50)
    : IQuery<IReadOnlyList<NotificationResponse>>;
