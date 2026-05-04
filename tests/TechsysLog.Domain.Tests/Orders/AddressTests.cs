using Shouldly;
using TechsysLog.Domain.Orders.ValueObjects;

namespace TechsysLog.Domain.Tests.Orders;

public sealed class AddressTests
{
    [Fact]
    public void Create_WithAllValidFields_ShouldSucceed()
    {
        var cep = Cep.Create("01310-100").Value;

        var result = Address.Create(
            cep: cep,
            street: "Av. Paulista",
            number: "1000",
            neighborhood: "Bela Vista",
            city: "São Paulo",
            state: "SP");

        result.IsSuccess.ShouldBeTrue();
        var addr = result.Value;
        addr.Cep.ShouldBe(cep);
        addr.Street.ShouldBe("Av. Paulista");
        addr.Number.ShouldBe("1000");
        addr.Neighborhood.ShouldBe("Bela Vista");
        addr.City.ShouldBe("São Paulo");
        addr.State.ShouldBe("SP");
    }

    [Fact]
    public void Create_WithNullCep_ShouldFail()
    {
        var result = Address.Create(
            cep: null!,
            street: "Av. Paulista",
            number: "1000",
            neighborhood: "Bela Vista",
            city: "São Paulo",
            state: "SP");

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AddressErrors.CepRequired);
    }

    [Theory]
    [InlineData("", "1000", "Bela Vista", "São Paulo", "SP")]
    [InlineData("Av. Paulista", "", "Bela Vista", "São Paulo", "SP")]
    [InlineData("Av. Paulista", "1000", "", "São Paulo", "SP")]
    [InlineData("Av. Paulista", "1000", "Bela Vista", "", "SP")]
    public void Create_WithEmptyMandatoryField_ShouldFail(
        string street, string number, string neighborhood, string city, string state)
    {
        var cep = Cep.Create("01310100").Value;

        var result = Address.Create(cep, street, number, neighborhood, city, state);

        result.IsFailure.ShouldBeTrue();
    }

    [Theory]
    [InlineData("S")]
    [InlineData("SAO")]
    [InlineData("123")]
    public void Create_WithInvalidStateFormat_ShouldFail(string state)
    {
        var cep = Cep.Create("01310100").Value;

        var result = Address.Create(cep, "Rua X", "1", "Bairro", "Cidade", state);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AddressErrors.InvalidState);
    }

    [Fact]
    public void Equality_SameComponents_ShouldBeEqual()
    {
        var cep = Cep.Create("01310100").Value;
        var a = Address.Create(cep, "X", "1", "Y", "Z", "SP").Value;
        var b = Address.Create(cep, "X", "1", "Y", "Z", "SP").Value;

        a.ShouldBe(b);
    }
}
