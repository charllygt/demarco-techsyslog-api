using TechsysLog.Domain.Common;

namespace TechsysLog.Application.Abstractions.Messaging;

public interface IDispatcher
{
    Task<Result> Send(ICommand command, CancellationToken ct);
    Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command, CancellationToken ct);
    Task<Result<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken ct);
}
