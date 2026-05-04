using TechsysLog.Domain.Orders.ValueObjects;

namespace TechsysLog.TestUtilities.Builders;

public sealed class AddressBuilder
{
    private string _cep = "01310100";
    private string _street = "Av. Paulista";
    private string _number = "1000";
    private string _neighborhood = "Bela Vista";
    private string _city = "São Paulo";
    private string _state = "SP";

    public static AddressBuilder New() => new();

    public AddressBuilder WithCep(string cep) { _cep = cep; return this; }
    public AddressBuilder WithStreet(string street) { _street = street; return this; }
    public AddressBuilder WithNumber(string number) { _number = number; return this; }
    public AddressBuilder WithNeighborhood(string n) { _neighborhood = n; return this; }
    public AddressBuilder WithCity(string city) { _city = city; return this; }
    public AddressBuilder WithState(string state) { _state = state; return this; }

    public Address Build()
    {
        var cep = Cep.Create(_cep).Value;
        return Address.Create(cep, _street, _number, _neighborhood, _city, _state).Value;
    }
}
