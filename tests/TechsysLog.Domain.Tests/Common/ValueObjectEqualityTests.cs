using Shouldly;
using TechsysLog.Domain.Common;

namespace TechsysLog.Domain.Tests.Common;

public sealed class ValueObjectEqualityTests
{
    private sealed class TestVo(string a, int b) : ValueObject
    {
        public string A { get; } = a;
        public int B { get; } = b;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return A;
            yield return B;
        }
    }

    private sealed class OtherVo(string a, int b) : ValueObject
    {
        public string A { get; } = a;
        public int B { get; } = b;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return A;
            yield return B;
        }
    }

    [Fact]
    public void TwoVOs_WithSameComponents_ShouldBeEqual()
    {
        var a = new TestVo("x", 1);
        var b = new TestVo("x", 1);

        a.ShouldBe(b);
        (a == b).ShouldBeTrue();
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoVOs_WithDifferentComponents_ShouldNotBeEqual()
    {
        var a = new TestVo("x", 1);
        var b = new TestVo("x", 2);

        a.ShouldNotBe(b);
        (a != b).ShouldBeTrue();
    }

    [Fact]
    public void VOs_OfDifferentTypes_ShouldNotBeEqual_EvenWithSameComponents()
    {
        var a = new TestVo("x", 1);
        var b = new OtherVo("x", 1);

        a.Equals(b).ShouldBeFalse();
    }

    [Fact]
    public void VO_ComparedWithNull_ShouldNotBeEqual()
    {
        var a = new TestVo("x", 1);

        a.Equals(null).ShouldBeFalse();
        (a == null).ShouldBeFalse();
        (null == a).ShouldBeFalse();
    }
}
