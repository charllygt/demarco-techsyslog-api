using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Messaging;
using TechsysLog.Domain.Common;

namespace TechsysLog.Application.Tests.Messaging;

public sealed class DispatcherTests
{
    public sealed record Ping : ICommand;
    public sealed record PingWithResult(string Input) : ICommand<string>;
    public sealed record PingQuery(string Input) : IQuery<string>;

    public sealed class PingHandler : ICommandHandler<Ping>
    {
        public Task<Result> Handle(Ping command, CancellationToken ct) =>
            Task.FromResult(Result.Success());
    }

    public sealed class PingWithResultHandler : ICommandHandler<PingWithResult, string>
    {
        public Task<Result<string>> Handle(PingWithResult command, CancellationToken ct) =>
            Task.FromResult(Result.Success($"echo:{command.Input}"));
    }

    public sealed class PingQueryHandler : IQueryHandler<PingQuery, string>
    {
        public Task<Result<string>> Handle(PingQuery query, CancellationToken ct) =>
            Task.FromResult(Result.Success($"q:{query.Input}"));
    }

    private static IDispatcher BuildDispatcher(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();
        configure(services);
        services.AddScoped<IDispatcher, Dispatcher>();
        return services.BuildServiceProvider().GetRequiredService<IDispatcher>();
    }

    [Fact]
    public async Task Send_VoidCommand_ShouldInvokeHandler()
    {
        var dispatcher = BuildDispatcher(s =>
            s.AddScoped<ICommandHandler<Ping>, PingHandler>());

        var result = await dispatcher.Send(new Ping(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Send_CommandWithResponse_ShouldReturnHandlerOutput()
    {
        var dispatcher = BuildDispatcher(s =>
            s.AddScoped<ICommandHandler<PingWithResult, string>, PingWithResultHandler>());

        var result = await dispatcher.Send(new PingWithResult("hi"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("echo:hi");
    }

    [Fact]
    public async Task Send_Query_ShouldReturnHandlerOutput()
    {
        var dispatcher = BuildDispatcher(s =>
            s.AddScoped<IQueryHandler<PingQuery, string>, PingQueryHandler>());

        var result = await dispatcher.Send(new PingQuery("x"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("q:x");
    }

    [Fact]
    public async Task Send_WithoutHandler_ShouldThrow()
    {
        var dispatcher = BuildDispatcher(_ => { });

        await Should.ThrowAsync<InvalidOperationException>(
            () => dispatcher.Send(new Ping(), CancellationToken.None));
    }
}
