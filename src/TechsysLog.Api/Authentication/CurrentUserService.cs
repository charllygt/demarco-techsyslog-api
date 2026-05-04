using System.Security.Claims;
using TechsysLog.Application.Abstractions.Authentication;
using TechsysLog.Domain.Common.Ids;

namespace TechsysLog.Api.Authentication;

internal sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUser
{
    public UserId? UserId
    {
        get
        {
            var sub = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? accessor.HttpContext?.User.FindFirstValue("sub");
            return Guid.TryParse(sub, out var guid) ? new UserId(guid) : null;
        }
    }

    public bool IsAuthenticated => UserId is not null;
}
