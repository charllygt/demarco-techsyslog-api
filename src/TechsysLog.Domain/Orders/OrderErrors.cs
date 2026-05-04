using TechsysLog.Domain.Common;

namespace TechsysLog.Domain.Orders;

public static class OrderErrors
{
    public static readonly Error NumberRequired = new("Order.NumberRequired", "Número do pedido é obrigatório.");
    public static readonly Error DescriptionRequired = new("Order.DescriptionRequired", "Descrição é obrigatória.");
    public static readonly Error ValueRequired = new("Order.ValueRequired", "Valor é obrigatório.");
    public static readonly Error AddressRequired = new("Order.AddressRequired", "Endereço de entrega é obrigatório.");
    public static readonly Error CreatorRequired = new("Order.CreatorRequired", "Usuário criador é obrigatório.");
    public static readonly Error AlreadyDelivered = new("Order.AlreadyDelivered", "Este pedido já foi entregue.");
    public static readonly Error DeliveryDateBeforeCreation = new("Order.DeliveryDateBeforeCreation", "Data de entrega não pode ser anterior à criação do pedido.");
    public static readonly Error NotFound = new("Order.NotFound", "Pedido não encontrado.");
}
