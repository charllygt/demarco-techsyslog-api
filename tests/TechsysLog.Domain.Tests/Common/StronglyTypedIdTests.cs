using Shouldly;
using TechsysLog.Domain.Common.Ids;

namespace TechsysLog.Domain.Tests.Common;

public sealed class StronglyTypedIdTests
{
    [Fact]
    public void New_ShouldGenerateNonEmptyGuid()
    {
        var id = UserId.New();

        id.Value.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void TwoIds_WithSameValue_ShouldBeEqual()
    {
        var guid = Guid.NewGuid();
        var a = new UserId(guid);
        var b = new UserId(guid);

        a.ShouldBe(b);
        (a == b).ShouldBeTrue();
    }

    [Fact]
    public void Ids_OfDifferentTypes_ShouldNotBeAssignableInterchangeably()
    {
        var userGuid = Guid.NewGuid();
        var userId = new UserId(userGuid);
        var orderId = new OrderId(userGuid);

        userId.GetType().ShouldNotBe(orderId.GetType());
    }
}
