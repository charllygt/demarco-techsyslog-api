using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Notifications;

namespace TechsysLog.Application.Abstractions.Persistence;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(NotificationId id, CancellationToken ct);
    Task AddAsync(Notification notification, CancellationToken ct);
    Task UpdateAsync(Notification notification, CancellationToken ct);
    Task<IReadOnlyList<Notification>> ListForUserAsync(UserId userId, int skip, int take, CancellationToken ct);
}
