using Microsoft.Extensions.DependencyInjection;
using TechsysLog.Application.Abstractions.Events;
using TechsysLog.Domain.Common;

namespace TechsysLog.Infrastructure.Events;

internal sealed class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var domainEvent in events)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                if (handler is null) continue;
                var method = handlerType.GetMethod("HandleAsync")!;
                var task = (Task)method.Invoke(handler, [domainEvent, ct])!;
                await task.ConfigureAwait(false);
            }
        }
    }
}
