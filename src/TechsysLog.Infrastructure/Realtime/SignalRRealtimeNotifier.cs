using System.Globalization;
using Microsoft.AspNetCore.SignalR;
using TechsysLog.Application.Abstractions.Realtime;

namespace TechsysLog.Infrastructure.Realtime;

internal sealed class SignalRRealtimeNotifier(
    IHubContext<NotificationHub, INotificationClient> hub) : IRealtimeNotifier
{
    public async Task NotifyAsync(RealtimeNotificationPayload payload, CancellationToken ct)
    {
        // Envia para cada destinatário via grupo nominal (UserId)
        foreach (var userId in payload.Recipients)
        {
            await hub.Clients.Group(userId.Value.ToString())
                .ReceiveNotification(payload)
                .ConfigureAwait(false);
        }
    }
}
