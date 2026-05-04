using Shouldly;
using TechsysLog.Domain.Common;

namespace TechsysLog.Domain.Tests.Common;

public sealed class AggregateRootTests
{
    private sealed record TestEvent(DateTime OccurredOn) : IDomainEvent;

    private sealed class TestAggregate(Guid id) : AggregateRoot<Guid>(id)
    {
        public void DoSomething() => Raise(new TestEvent(DateTime.UtcNow));
    }

    [Fact]
    public void NewAggregate_ShouldHaveNoDomainEvents()
    {
        var agg = new TestAggregate(Guid.NewGuid());

        agg.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void Raise_ShouldAddEventToCollection()
    {
        var agg = new TestAggregate(Guid.NewGuid());

        agg.DoSomething();
        agg.DoSomething();

        agg.DomainEvents.Count.ShouldBe(2);
    }

    [Fact]
    public void ClearDomainEvents_ShouldEmptyCollection()
    {
        var agg = new TestAggregate(Guid.NewGuid());
        agg.DoSomething();

        agg.ClearDomainEvents();

        agg.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void Entities_WithSameId_ShouldBeEqual()
    {
        var id = Guid.NewGuid();
        var a = new TestAggregate(id);
        var b = new TestAggregate(id);

        a.Equals(b).ShouldBeTrue();
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }
}
