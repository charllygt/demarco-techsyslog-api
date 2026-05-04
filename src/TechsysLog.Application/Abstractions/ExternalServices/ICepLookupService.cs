using TechsysLog.Domain.Common;

namespace TechsysLog.Application.Abstractions.ExternalServices;

public interface ICepLookupService
{
    Task<Result<CepLookupResult>> LookupAsync(string cep, CancellationToken ct);
}

public sealed record CepLookupResult(
    string Cep,
    string Street,
    string Neighborhood,
    string City,
    string State);
