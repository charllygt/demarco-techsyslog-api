using TechsysLog.Domain.Common;

namespace TechsysLog.Application.Common;

public static class ResultExtensions
{
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map) =>
        result.IsSuccess
            ? Result.Success(map(result.Value))
            : Result.Failure<TOut>(result.Error);

    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(
        this Task<Result<TIn>> resultTask,
        Func<TIn, TOut> map)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Map(map);
    }
}
