using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechsysLog.Application.Abstractions.Authentication;
using TechsysLog.Application.Abstractions.Messaging;
using TechsysLog.Application.Orders.CreateOrder;
using TechsysLog.Application.Orders.GetOrderById;
using TechsysLog.Application.Orders.ListOrders;
using TechsysLog.Application.Orders.RegisterDelivery;
using TechsysLog.Domain.Common.Ids;

namespace TechsysLog.Api.Controllers;

[Authorize]
public sealed class OrdersController(IDispatcher dispatcher, ICurrentUser currentUser) : ApiControllerBase
{
    public sealed record CreateOrderRequest(string Description, decimal Value, AddressDto ShippingAddress);
    public sealed record RegisterDeliveryRequest(DateTime DeliveredAtUtc);

    [HttpPost]
    [SwaggerOperation(Summary = "Cria um novo pedido")]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        if (currentUser.UserId is null) return Unauthorized();
        var command = new CreateOrderCommand(request.Description, request.Value, request.ShippingAddress, currentUser.UserId);
        var result = await dispatcher.Send(command, ct);
        return ToActionResult(result, StatusCodes.Status201Created);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Busca pedido por ID")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.Send(new GetOrderByIdQuery(new OrderId(id)), ct);
        return ToActionResult(result);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Lista pedidos paginados")]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, CancellationToken ct = default)
    {
        var result = await dispatcher.Send(new ListOrdersQuery(skip, take), ct);
        return ToActionResult(result);
    }

    [HttpPost("{id:guid}/deliveries")]
    [SwaggerOperation(Summary = "Registra entrega de um pedido")]
    public async Task<IActionResult> RegisterDelivery(Guid id, [FromBody] RegisterDeliveryRequest request, CancellationToken ct)
    {
        var command = new RegisterDeliveryCommand(new OrderId(id), request.DeliveredAtUtc);
        var result = await dispatcher.Send(command, ct);
        return ToActionResult(result, StatusCodes.Status204NoContent);
    }
}
