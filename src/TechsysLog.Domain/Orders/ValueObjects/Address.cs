using TechsysLog.Domain.Common;

namespace TechsysLog.Domain.Orders.ValueObjects;

public sealed class Address : ValueObject
{
    public Cep Cep { get; }
    public string Street { get; }
    public string Number { get; }
    public string Neighborhood { get; }
    public string City { get; }
    public string State { get; }

    private Address(Cep cep, string street, string number, string neighborhood, string city, string state)
    {
        Cep = cep;
        Street = street;
        Number = number;
        Neighborhood = neighborhood;
        City = city;
        State = state;
    }

    public static Result<Address> Create(
        Cep cep, string? street, string? number,
        string? neighborhood, string? city, string? state)
    {
        if (cep is null)
            return Result.Failure<Address>(AddressErrors.CepRequired);

        if (string.IsNullOrWhiteSpace(street))
            return Result.Failure<Address>(AddressErrors.StreetRequired);

        if (string.IsNullOrWhiteSpace(number))
            return Result.Failure<Address>(AddressErrors.NumberRequired);

        if (string.IsNullOrWhiteSpace(neighborhood))
            return Result.Failure<Address>(AddressErrors.NeighborhoodRequired);

        if (string.IsNullOrWhiteSpace(city))
            return Result.Failure<Address>(AddressErrors.CityRequired);

        if (string.IsNullOrWhiteSpace(state) || state.Trim().Length != 2 || !state.All(char.IsLetter))
            return Result.Failure<Address>(AddressErrors.InvalidState);

        return Result.Success(new Address(
            cep,
            street.Trim(),
            number.Trim(),
            neighborhood.Trim(),
            city.Trim(),
            state.Trim().ToUpperInvariant()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Cep;
        yield return Street;
        yield return Number;
        yield return Neighborhood;
        yield return City;
        yield return State;
    }
}

public static class AddressErrors
{
    public static readonly Error CepRequired = new("Address.CepRequired", "CEP é obrigatório.");
    public static readonly Error StreetRequired = new("Address.StreetRequired", "Logradouro é obrigatório.");
    public static readonly Error NumberRequired = new("Address.NumberRequired", "Número é obrigatório.");
    public static readonly Error NeighborhoodRequired = new("Address.NeighborhoodRequired", "Bairro é obrigatório.");
    public static readonly Error CityRequired = new("Address.CityRequired", "Cidade é obrigatória.");
    public static readonly Error InvalidState = new("Address.InvalidState", "Estado deve ser uma sigla UF de 2 letras.");
}
