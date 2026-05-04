using Shouldly;
using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Notifications;
using TechsysLog.Domain.Notifications.Enums;
using TechsysLog.TestUtilities.Doubles;

namespace TechsysLog.Domain.Tests.Notifications;

public sealed class NotificationTests
{
    private static FixedDateTimeProvider Clock => FixedDateTimeProvider.AtUtc(2026, 4, 30, 10, 0, 0);

    [Fact]
    public void CreateForUsers_WithValidData_ShouldSucceed()
    {
        var u1 = UserId.New();
        var u2 = UserId.New();
        var clock = Clock;

        var result = Notification.CreateForUsers(
            NotificationType.OrderCreated, "Pedido criado", "ORD-001 foi criado", new[] { u1, u2 }, clock);

        result.IsSuccess.ShouldBeTrue();
        var n = result.Value;
        n.Type.ShouldBe(NotificationType.OrderCreated);
        n.Title.ShouldBe("Pedido criado");
        n.Message.ShouldBe("ORD-001 foi criado");
        n.CreatedAt.ShouldBe(clock.UtcNow);
        n.Recipients.Count.ShouldBe(2);
        n.Recipients.ShouldAllBe(r => r.ReadAt == null);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreateForUsers_WithEmptyTitle_ShouldFail(string? title)
    {
        var result = Notification.CreateForUsers(
            NotificationType.OrderCreated, title!, "msg", new[] { UserId.New() }, Clock);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(NotificationErrors.TitleRequired);
    }

    [Fact]
    public void CreateForUsers_WithEmptyMessage_ShouldFail()
    {
        var result = Notification.CreateForUsers(
            NotificationType.OrderCreated, "title", "", new[] { UserId.New() }, Clock);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(NotificationErrors.MessageRequired);
    }

    [Fact]
    public void CreateForUsers_WithEmptyRecipients_ShouldFail()
    {
        var result = Notification.CreateForUsers(
            NotificationType.OrderCreated, "title", "msg", Array.Empty<UserId>(), Clock);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(NotificationErrors.RecipientsRequired);
    }

    [Fact]
    public void MarkAsReadBy_RecipientUser_ShouldUpdateReadAt()
    {
        var clock = Clock;
        var u1 = UserId.New();
        var notification = Notification.CreateForUsers(
            NotificationType.OrderCreated, "title", "msg", new[] { u1 }, clock).Value;
        var readAt = clock.UtcNow.AddMinutes(5);

        var result = notification.MarkAsReadBy(u1, readAt);

        result.IsSuccess.ShouldBeTrue();
        notification.Recipients.First(r => r.UserId == u1).ReadAt.ShouldBe(readAt);
    }

    [Fact]
    public void MarkAsReadBy_AlreadyReadRecipient_ShouldBeIdempotent()
    {
        var clock = Clock;
        var u1 = UserId.New();
        var notification = Notification.CreateForUsers(
            NotificationType.OrderCreated, "title", "msg", new[] { u1 }, clock).Value;
        var firstRead = clock.UtcNow.AddMinutes(5);
        notification.MarkAsReadBy(u1, firstRead);

        var result = notification.MarkAsReadBy(u1, clock.UtcNow.AddMinutes(10));

        result.IsSuccess.ShouldBeTrue();
        notification.Recipients.First(r => r.UserId == u1).ReadAt.ShouldBe(firstRead);
    }

    [Fact]
    public void MarkAsReadBy_NonRecipientUser_ShouldFail()
    {
        var clock = Clock;
        var notification = Notification.CreateForUsers(
            NotificationType.OrderCreated, "title", "msg", new[] { UserId.New() }, clock).Value;

        var result = notification.MarkAsReadBy(UserId.New(), clock.UtcNow);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(NotificationErrors.NotARecipient);
    }
}
