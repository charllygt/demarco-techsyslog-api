using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechsysLog.Application.Abstractions.Authentication;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Notifications.ListMyNotifications;
using TechsysLog.Application.Notifications.MarkNotificationAsRead;
using TechsysLog.Domain.Common.Ids;

namespace TechsysLog.Api.Controllers;

[Authorize]
public sealed class NotificationsController(IDispatcher dispatcher, ICurrentUser currentUser) : ApiControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lista minhas notificações")]
    public async Task<IActionResult> ListMine([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
    {
        if (currentUser.UserId is null) return Unauthorized();
        var result = await dispatcher.Send(new ListMyNotificationsQuery(currentUser.UserId, skip, take), ct);
        return ToActionResult(result);
    }

    [HttpPatch("{id:guid}/read")]
    [SwaggerOperation(Summary = "Marca notificação como lida")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct)
    {
        if (currentUser.UserId is null) return Unauthorized();
        var result = await dispatcher.Send(new MarkAsReadCommand(new NotificationId(id), currentUser.UserId), ct);
        return ToActionResult(result, StatusCodes.Status204NoContent);
    }
}
