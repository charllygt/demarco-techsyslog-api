using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TechsysLog.Application.Abstractions.ExternalServices;
using TechsysLog.Domain.Common;

namespace TechsysLog.Infrastructure.ExternalServices.ViaCep;

internal sealed class ViaCepClient(
    HttpClient httpClient,
    IMemoryCache cache,
    ILogger<ViaCepClient> logger) : ICepLookupService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public async Task<Result<CepLookupResult>> LookupAsync(string cep, CancellationToken ct)
    {
        var normalized = (cep ?? string.Empty).Trim().Replace("-", string.Empty, StringComparison.Ordinal);
        if (normalized.Length != 8 || !normalized.All(char.IsDigit))
            return Result.Failure<CepLookupResult>(CepLookupErrors.InvalidFormat);

        var cacheKey = $"cep:{normalized}";
        if (cache.TryGetValue<CepLookupResult>(cacheKey, out var cached) && cached is not null)
            return Result.Success(cached);

        try
        {
            var url = string.Create(CultureInfo.InvariantCulture, $"{normalized}/json/");
            var response = await httpClient.GetFromJsonAsync<ViaCepResponse>(url, ct).ConfigureAwait(false);

            if (response is null || response.Erro)
                return Result.Failure<CepLookupResult>(CepLookupErrors.NotFound);

            var result = new CepLookupResult(
                Cep: normalized,
                Street: response.Logradouro,
                Neighborhood: response.Bairro,
                City: response.Localidade,
                State: response.Uf);

            cache.Set(cacheKey, result, CacheTtl);
            return Result.Success(result);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Falha ao consultar ViaCEP {Cep}", normalized);
            return Result.Failure<CepLookupResult>(CepLookupErrors.LookupFailed);
        }
        catch (TaskCanceledException ex)
        {
            logger.LogWarning(ex, "Timeout ao consultar ViaCEP {Cep}", normalized);
            return Result.Failure<CepLookupResult>(CepLookupErrors.LookupFailed);
        }
    }
}

public static class CepLookupErrors
{
    public static readonly Error InvalidFormat = new("Cep.InvalidFormat", "CEP inválido.");
    public static readonly Error NotFound = new("Cep.NotFound", "CEP não encontrado.");
    public static readonly Error LookupFailed = new("Cep.LookupFailed", "Falha ao consultar serviço de CEP.");
}
