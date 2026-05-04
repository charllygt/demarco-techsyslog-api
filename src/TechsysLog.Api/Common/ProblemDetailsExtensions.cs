using Microsoft.AspNetCore.Mvc;
using TechsysLog.Domain.Common;

namespace TechsysLog.Api.Common;

internal static class ProblemDetailsExtensions
{
    public static IActionResult ToActionResult(this Error error, ControllerBase controller)
    {
        var (status, title) = ResolveStatus(error);
        var problem = new ProblemDetails
        {
            Title = title,
            Detail = error.Description,
            Status = status,
            Type = $"https://techsyslog/errors/{error.Code}"
        };
        problem.Extensions["code"] = error.Code;
        return controller.StatusCode(status, problem);
    }

    private static (int Status, string Title) ResolveStatus(Error error) => error.Code switch
    {
        var c when c.StartsWith("Validation.", StringComparison.Ordinal) => (StatusCodes.Status400BadRequest, "Erro de validação"),
        "Application.Validation" => (StatusCodes.Status400BadRequest, "Erro de validação"),
        "Application.Unauthorized" or "User.InvalidCredentials" => (StatusCodes.Status401Unauthorized, "Não autenticado"),
        "Application.Forbidden" => (StatusCodes.Status403Forbidden, "Sem permissão"),
        "User.EmailAlreadyInUse" or "Application.Conflict" or "Order.AlreadyDelivered" => (StatusCodes.Status409Conflict, "Conflito"),
        "User.NotFound" or "Order.NotFound" or "Notification.NotFound" or "Cep.NotFound" => (StatusCodes.Status404NotFound, "Não encontrado"),
        _ => (StatusCodes.Status400BadRequest, "Erro")
    };
}
