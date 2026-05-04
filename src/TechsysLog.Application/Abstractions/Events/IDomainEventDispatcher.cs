using TechsysLog.Domain.Common;

namespace TechsysLog.Application.Abstractions.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct);
}
