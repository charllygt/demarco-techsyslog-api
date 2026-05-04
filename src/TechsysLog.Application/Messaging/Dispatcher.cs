using Microsoft.Extensions.DependencyInjection;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Domain.Common;

namespace TechsysLog.Application.Messaging;

internal sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    public async Task<Result> Send(ICommand command, CancellationToken ct)
    {
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"Handler não registrado para {command.GetType().Name}.");

        var method = handlerType.GetMethod(nameof(ICommandHandler<ICommand>.Handle))!;
        var task = (Task<Result>)method.Invoke(handler, [command, ct])!;
        return await task.ConfigureAwait(false);
    }

    public Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command, CancellationToken ct)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        return InvokeHandler<TResponse>(handlerType, command, ct);
    }

    public Task<Result<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken ct)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        return InvokeHandler<TResponse>(handlerType, query, ct);
    }

    private async Task<Result<TResponse>> InvokeHandler<TResponse>(Type handlerType, object request, CancellationToken ct)
    {
        var handler = serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"Handler não registrado para {request.GetType().Name}.");

        var method = handlerType.GetMethod("Handle")!;
        var task = (Task<Result<TResponse>>)method.Invoke(handler, [request, ct])!;
        return await task.ConfigureAwait(false);
    }
}
