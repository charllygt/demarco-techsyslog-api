namespace TechsysLog.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
