using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Users.CreateUser;

namespace TechsysLog.Api.Controllers;

public sealed class UsersController(IDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Cadastra novo usuário")]
    [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(CreateUserResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command, CancellationToken ct)
    {
        var result = await dispatcher.Send(command, ct);
        return ToActionResult(result, StatusCodes.Status201Created);
    }
}
