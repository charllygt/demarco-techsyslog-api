using System.Text.RegularExpressions;
using TechsysLog.Domain.Common;

namespace TechsysLog.Domain.Users.ValueObjects;

public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private const int MaxLength = 320;

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result.Failure<Email>(EmailErrors.Empty);

        var normalized = input.Trim().ToLowerInvariant();

        if (normalized.Length > MaxLength)
            return Result.Failure<Email>(EmailErrors.TooLong);

        if (!EmailRegex.IsMatch(normalized))
            return Result.Failure<Email>(EmailErrors.InvalidFormat);

        return Result.Success(new Email(normalized));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

public static class EmailErrors
{
    public static readonly Error Empty = new("Email.Empty", "Email não pode ser vazio.");
    public static readonly Error TooLong = new("Email.TooLong", "Email não pode exceder 320 caracteres.");
    public static readonly Error InvalidFormat = new("Email.InvalidFormat", "Email com formato inválido.");
}
