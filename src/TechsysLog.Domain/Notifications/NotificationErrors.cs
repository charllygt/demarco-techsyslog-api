using TechsysLog.Domain.Common;

namespace TechsysLog.Domain.Notifications;

public static class NotificationErrors
{
    public static readonly Error TitleRequired = new("Notification.TitleRequired", "Título é obrigatório.");
    public static readonly Error MessageRequired = new("Notification.MessageRequired", "Mensagem é obrigatória.");
    public static readonly Error RecipientsRequired = new("Notification.RecipientsRequired", "Lista de destinatários não pode estar vazia.");
    public static readonly Error NotARecipient = new("Notification.NotARecipient", "Usuário não é destinatário desta notificação.");
    public static readonly Error NotFound = new("Notification.NotFound", "Notificação não encontrada.");
}
