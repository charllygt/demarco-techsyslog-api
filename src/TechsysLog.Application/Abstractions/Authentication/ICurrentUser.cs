using TechsysLog.Domain.Common.Ids;

namespace TechsysLog.Application.Abstractions.Authentication;

public interface ICurrentUser
{
    UserId? UserId { get; }
    bool IsAuthenticated { get; }
}
