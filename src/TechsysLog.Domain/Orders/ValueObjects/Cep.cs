using System.Text.RegularExpressions;
using TechsysLog.Domain.Common;

namespace TechsysLog.Domain.Orders.ValueObjects;

public sealed class Cep : ValueObject
{
    private static readonly Regex DigitsOnlyRegex =
        new(@"^\d{8}$", RegexOptions.Compiled);

    public string Value { get; }
    public string Formatted => $"{Value[..5]}-{Value[5..]}";

    private Cep(string value) => Value = value;

    public static Result<Cep> Create(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result.Failure<Cep>(CepErrors.Empty);

        var normalized = input.Trim().Replace("-", string.Empty, StringComparison.Ordinal);

        if (!DigitsOnlyRegex.IsMatch(normalized))
            return Result.Failure<Cep>(CepErrors.InvalidFormat);

        return Result.Success(new Cep(normalized));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Formatted;
}

public static class CepErrors
{
    public static readonly Error Empty = new("Cep.Empty", "CEP não pode ser vazio.");
    public static readonly Error InvalidFormat = new("Cep.InvalidFormat", "CEP deve conter exatamente 8 dígitos numéricos.");
}
