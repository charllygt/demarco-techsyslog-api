using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Domain.Common;
using TechsysLog.Domain.Notifications;

namespace TechsysLog.Application.Notifications.MarkNotificationAsRead;

internal sealed class MarkAsReadCommandHandler(
    INotificationRepository notifications,
    IDateTimeProvider clock) : ICommandHandler<MarkAsReadCommand>
{
    public async Task<Result> Handle(MarkAsReadCommand command, CancellationToken ct)
    {
        var notification = await notifications.GetByIdAsync(command.NotificationId, ct).ConfigureAwait(false);
        if (notification is null) return Result.Failure(NotificationErrors.NotFound);

        var result = notification.MarkAsReadBy(command.UserId, clock.UtcNow);
        if (result.IsFailure) return result;

        await notifications.UpdateAsync(notification, ct).ConfigureAwait(false);
        return Result.Success();
    }
}
