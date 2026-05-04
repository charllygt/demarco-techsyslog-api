using TechsysLog.Domain.Common;

namespace TechsysLog.Domain.Orders.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string? currency = "BRL")
    {
        if (amount < 0)
            return Result.Failure<Money>(MoneyErrors.NegativeAmount);

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            return Result.Failure<Money>(MoneyErrors.InvalidCurrency);

        return Result.Success(new Money(amount, currency.ToUpperInvariant()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";
}

public static class MoneyErrors
{
    public static readonly Error NegativeAmount = new("Money.NegativeAmount", "Valor monetário não pode ser negativo.");
    public static readonly Error InvalidCurrency = new("Money.InvalidCurrency", "Moeda inválida (formato esperado: ISO 4217 com 3 letras).");
}
