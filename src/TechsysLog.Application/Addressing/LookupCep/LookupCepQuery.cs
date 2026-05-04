using TechsysLog.Application.Abstractions.ExternalServices;
using TechsysLog.Application.Abstractions.Messaging;

namespace TechsysLog.Application.Addressing.LookupCep;

public sealed record LookupCepQuery(string Cep) : IQuery<CepLookupResult>;
