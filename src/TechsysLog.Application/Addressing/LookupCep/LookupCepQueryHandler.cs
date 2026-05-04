using TechsysLog.Application.Abstractions.ExternalServices;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Domain.Common;

namespace TechsysLog.Application.Addressing.LookupCep;

internal sealed class LookupCepQueryHandler(ICepLookupService service)
    : IQueryHandler<LookupCepQuery, CepLookupResult>
{
    public Task<Result<CepLookupResult>> Handle(LookupCepQuery query, CancellationToken ct) =>
        service.LookupAsync(query.Cep, ct);
}
