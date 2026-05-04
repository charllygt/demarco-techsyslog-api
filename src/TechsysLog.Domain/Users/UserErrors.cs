using TechsysLog.Domain.Common;

namespace TechsysLog.Domain.Users;

public static class UserErrors
{
    public static readonly Error NameRequired = new("User.NameRequired", "Nome do usuário é obrigatório.");
    public static readonly Error NameTooLong = new("User.NameTooLong", "Nome não pode exceder 150 caracteres.");
    public static readonly Error EmailRequired = new("User.EmailRequired", "Email é obrigatório.");
    public static readonly Error PasswordHashRequired = new("User.PasswordHashRequired", "Hash de senha é obrigatório.");
    public static readonly Error NotFound = new("User.NotFound", "Usuário não encontrado.");
    public static readonly Error EmailAlreadyInUse = new("User.EmailAlreadyInUse", "Email já cadastrado.");
    public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Credenciais inválidas.");
}
