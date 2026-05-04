using TechsysLog.Application.Abstractions.Realtime;

namespace TechsysLog.Infrastructure.Realtime;

public interface INotificationClient
{
    Task ReceiveNotification(RealtimeNotificationPayload payload);
}
