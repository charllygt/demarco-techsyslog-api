using TechsysLog.Domain.Common;

namespace TechsysLog.Application.Common;

public static class ApplicationErrors
{
    public static readonly Error Validation = new("Application.Validation", "Um ou mais erros de validação ocorreram.");
    public static readonly Error Unauthorized = new("Application.Unauthorized", "Não autenticado.");
    public static readonly Error Forbidden = new("Application.Forbidden", "Sem permissão para esta operação.");
    public static readonly Error Conflict = new("Application.Conflict", "A operação não pode ser concluída devido a um conflito de estado.");
    public static readonly Error Internal = new("Application.Internal", "Erro interno ao processar a requisição.");

    public static Error ValidationFailure(string field, string message) =>
        new($"Validation.{field}", message);
}
