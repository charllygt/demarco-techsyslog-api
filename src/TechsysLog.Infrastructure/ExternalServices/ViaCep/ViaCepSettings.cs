namespace TechsysLog.Infrastructure.ExternalServices.ViaCep;

public sealed class ViaCepSettings
{
    public const string SectionName = "ViaCep";

    public string BaseUrl { get; set; } = "https://viacep.com.br/ws/";
}
