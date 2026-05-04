using Shouldly;
using TechsysLog.Domain.Common;

namespace TechsysLog.Domain.Tests.Common;

public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldHaveIsSuccessTrueAndNoError()
    {
        var result = Result.Success();

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Error.ShouldBe(Error.None);
    }

    [Fact]
    public void Failure_ShouldHaveIsSuccessFalseAndProvidedError()
    {
        var error = new Error("Test.Error", "Something went wrong.");

        var result = Result.Failure(error);

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void GenericSuccess_ShouldExposeValue()
    {
        var result = Result.Success(42);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void GenericFailure_ShouldThrowOnValueAccess()
    {
        var result = Result.Failure<int>(new Error("X", "Y"));

        Should.Throw<InvalidOperationException>(() => _ = result.Value);
    }

    [Fact]
    public void Constructor_WithSuccessAndError_ShouldThrow()
    {
        Should.Throw<InvalidOperationException>(() => Result.Failure(Error.None));
    }
}
