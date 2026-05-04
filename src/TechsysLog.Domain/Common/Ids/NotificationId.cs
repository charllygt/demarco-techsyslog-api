namespace TechsysLog.Domain.Common.Ids;

public sealed record NotificationId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public static NotificationId New() => new(Guid.NewGuid());
}
