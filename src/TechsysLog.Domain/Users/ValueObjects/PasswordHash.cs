using TechsysLog.Domain.Common;

namespace TechsysLog.Domain.Users.ValueObjects;

public sealed class PasswordHash : ValueObject
{
    public string Value { get; }

    private PasswordHash(string value) => Value = value;

    public static Result<PasswordHash> Create(string? hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return Result.Failure<PasswordHash>(PasswordHashErrors.Empty);

        return Result.Success(new PasswordHash(hash));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    // ToString não expõe o hash — segurança contra log acidental.
    public override string ToString() => "***";
}

public static class PasswordHashErrors
{
    public static readonly Error Empty = new("PasswordHash.Empty", "Hash de senha não pode ser vazio.");
}
