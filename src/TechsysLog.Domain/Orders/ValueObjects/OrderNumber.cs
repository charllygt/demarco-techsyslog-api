using System.Globalization;
using System.Text.RegularExpressions;
using TechsysLog.Domain.Common;

namespace TechsysLog.Domain.Orders.ValueObjects;

public sealed class OrderNumber : ValueObject
{
    private static readonly Regex Format =
        new(@"^ORD-\d{6}-\d{6}$", RegexOptions.Compiled);

    public string Value { get; }

    private OrderNumber(string value) => Value = value;

    public static Result<OrderNumber> Create(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result.Failure<OrderNumber>(OrderNumberErrors.Empty);

        if (!Format.IsMatch(input))
            return Result.Failure<OrderNumber>(OrderNumberErrors.InvalidFormat);

        return Result.Success(new OrderNumber(input));
    }

    public static OrderNumber Generate(int year, int month, int sequential)
    {
        var formatted = string.Create(CultureInfo.InvariantCulture, $"ORD-{year:0000}{month:00}-{sequential:000000}");
        return new OrderNumber(formatted);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

public static class OrderNumberErrors
{
    public static readonly Error Empty = new("OrderNumber.Empty", "Número do pedido não pode ser vazio.");
    public static readonly Error InvalidFormat = new("OrderNumber.InvalidFormat",
        "Número do pedido deve seguir o formato ORD-YYYYMM-NNNNNN.");
}
