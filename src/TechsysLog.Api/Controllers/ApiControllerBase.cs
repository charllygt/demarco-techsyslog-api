using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TechsysLog.Api.Common;
using TechsysLog.Domain.Common;

namespace TechsysLog.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ToActionResult(Result result, int successStatusCode = StatusCodes.Status200OK) =>
        result.IsSuccess
            ? StatusCode(successStatusCode)
            : result.Error.ToActionResult(this);

    protected IActionResult ToActionResult<T>(Result<T> result, int successStatusCode = StatusCodes.Status200OK) =>
        result.IsSuccess
            ? StatusCode(successStatusCode, result.Value)
            : result.Error.ToActionResult(this);
}
