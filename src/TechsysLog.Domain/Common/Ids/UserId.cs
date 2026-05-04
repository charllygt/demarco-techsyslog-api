namespace TechsysLog.Domain.Common.Ids;

public sealed record UserId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public static UserId New() => new(Guid.NewGuid());
}
