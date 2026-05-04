using System.Reflection;
using FluentValidation;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Common;
using TechsysLog.Domain.Common;

namespace TechsysLog.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var activeValidators = validators.ToList();
        if (activeValidators.Count == 0)
            return await next().ConfigureAwait(false);

        var context = new ValidationContext<TRequest>(request);
        var failures = new List<FluentValidation.Results.ValidationFailure>();
        foreach (var validator in activeValidators)
        {
            var result = await validator.ValidateAsync(context, ct).ConfigureAwait(false);
            failures.AddRange(result.Errors.Where(f => f is not null));
        }

        if (failures.Count == 0)
            return await next().ConfigureAwait(false);

        var first = failures[0];
        var error = ApplicationErrors.ValidationFailure(first.PropertyName, first.ErrorMessage);
        return BuildFailureResponse(error);
    }

    private static TResponse BuildFailureResponse(Error error)
    {
        var responseType = typeof(TResponse);

        if (responseType == typeof(Result))
            return (TResponse)(object)Result.Failure(error);

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var failureMethod = typeof(Result)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == nameof(Result.Failure)
                            && m.IsGenericMethod
                            && m.GetParameters().Length == 1)
                .MakeGenericMethod(valueType);
            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        throw new InvalidOperationException(
            $"ValidationBehavior só suporta TResponse Result/Result<T>, recebido {responseType.Name}.");
    }
}
