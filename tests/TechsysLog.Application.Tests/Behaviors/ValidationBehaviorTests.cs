using FluentValidation;
using Shouldly;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Behaviors;
using TechsysLog.Domain.Common;

namespace TechsysLog.Application.Tests.Behaviors;

public sealed class ValidationBehaviorTests
{
    public sealed record TestCommand(string Name) : ICommand;

    public sealed class TestCommandValidator : AbstractValidator<TestCommand>
    {
        public TestCommandValidator()
        {
            RuleFor(c => c.Name).NotEmpty().WithMessage("Nome obrigatório");
        }
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCallNext()
    {
        var behavior = new ValidationBehavior<TestCommand, Result>(
            [new TestCommandValidator()]);
        var nextCalled = false;
        Task<Result> Next() { nextCalled = true; return Task.FromResult(Result.Success()); }

        var result = await behavior.Handle(new TestCommand("ok"), Next, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        nextCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WithInvalidRequest_ShouldShortCircuitWithFailure()
    {
        var behavior = new ValidationBehavior<TestCommand, Result>(
            [new TestCommandValidator()]);
        var nextCalled = false;
        Task<Result> Next() { nextCalled = true; return Task.FromResult(Result.Success()); }

        var result = await behavior.Handle(new TestCommand(""), Next, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldStartWith("Validation.");
        nextCalled.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WithoutValidators_ShouldCallNext()
    {
        var behavior = new ValidationBehavior<TestCommand, Result>([]);
        var nextCalled = false;
        Task<Result> Next() { nextCalled = true; return Task.FromResult(Result.Success()); }

        await behavior.Handle(new TestCommand(""), Next, CancellationToken.None);

        nextCalled.ShouldBeTrue();
    }
}
