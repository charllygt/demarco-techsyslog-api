using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Addressing.LookupCep;

namespace TechsysLog.Api.Controllers;

[Authorize]
public sealed class CepController(IDispatcher dispatcher) : ApiControllerBase
{
    [HttpGet("{cep}")]
    [SwaggerOperation(Summary = "Consulta endereço por CEP via ViaCEP")]
    public async Task<IActionResult> Lookup(string cep, CancellationToken ct)
    {
        var result = await dispatcher.Send(new LookupCepQuery(cep), ct);
        return ToActionResult(result);
    }
}
