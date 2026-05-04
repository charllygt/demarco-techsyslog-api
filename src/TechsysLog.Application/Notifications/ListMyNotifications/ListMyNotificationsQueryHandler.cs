using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Application.Notifications.Common;
using TechsysLog.Domain.Common;

namespace TechsysLog.Application.Notifications.ListMyNotifications;

internal sealed class ListMyNotificationsQueryHandler(INotificationRepository notifications)
    : IQueryHandler<ListMyNotificationsQuery, IReadOnlyList<NotificationResponse>>
{
    public async Task<Result<IReadOnlyList<NotificationResponse>>> Handle(
        ListMyNotificationsQuery query, CancellationToken ct)
    {
        var list = await notifications.ListForUserAsync(query.UserId, query.Skip, query.Take, ct).ConfigureAwait(false);

        IReadOnlyList<NotificationResponse> response = list.Select(n =>
        {
            var recipient = n.Recipients.First(r => r.UserId == query.UserId);
            return new NotificationResponse(
                n.Id.Value, n.Type.ToString(), n.Title, n.Message, n.CreatedAt, recipient.ReadAt);
        }).ToList();

        return Result.Success(response);
    }
}
