using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Users.Login;

namespace TechsysLog.Api.Controllers;

[AllowAnonymous]
public sealed class AuthController(IDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost("login")]
    [SwaggerOperation(Summary = "Autentica e retorna JWT")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await dispatcher.Send(command, ct);
        return ToActionResult(result);
    }
}
