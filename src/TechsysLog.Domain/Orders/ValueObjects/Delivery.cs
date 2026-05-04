using TechsysLog.Domain.Common;

namespace TechsysLog.Domain.Orders.ValueObjects;

public sealed class Delivery : ValueObject
{
    public DateTime DeliveredAt { get; }

    private Delivery(DateTime deliveredAt) => DeliveredAt = deliveredAt;

    public static Result<Delivery> Create(DateTime deliveredAt)
    {
        if (deliveredAt.Kind != DateTimeKind.Utc)
            return Result.Failure<Delivery>(DeliveryErrors.DateMustBeUtc);

        return Result.Success(new Delivery(deliveredAt));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return DeliveredAt;
    }
}

public static class DeliveryErrors
{
    public static readonly Error DateMustBeUtc = new("Delivery.DateMustBeUtc", "Data de entrega deve estar em UTC.");
}
