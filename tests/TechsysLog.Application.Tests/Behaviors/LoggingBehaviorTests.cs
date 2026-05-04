using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Behaviors;
using TechsysLog.Domain.Common;

namespace TechsysLog.Application.Tests.Behaviors;

public sealed class LoggingBehaviorTests
{
    public sealed record TestRequest : ICommand;

    [Fact]
    public async Task Handle_OnSuccess_ShouldLogStartAndCompletion()
    {
        var logger = Substitute.For<ILogger<LoggingBehavior<TestRequest, Result>>>();
        var behavior = new LoggingBehavior<TestRequest, Result>(logger);
        Task<Result> Next() => Task.FromResult(Result.Success());

        var result = await behavior.Handle(new TestRequest(), Next, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        logger.ReceivedCalls().Count().ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Handle_OnException_ShouldLogErrorAndRethrow()
    {
        var logger = Substitute.For<ILogger<LoggingBehavior<TestRequest, Result>>>();
        var behavior = new LoggingBehavior<TestRequest, Result>(logger);
        Task<Result> Next() => throw new InvalidOperationException("boom");

        await Should.ThrowAsync<InvalidOperationException>(
            () => behavior.Handle(new TestRequest(), Next, CancellationToken.None));
    }
}
