using FluentValidation;

namespace TechsysLog.Application.Orders.CreateOrder;

internal sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(c => c.Description).NotEmpty();
        RuleFor(c => c.Value).GreaterThanOrEqualTo(0);
        RuleFor(c => c.ShippingAddress).NotNull();
        RuleFor(c => c.ShippingAddress!.Cep).NotEmpty().When(c => c.ShippingAddress is not null);
        RuleFor(c => c.ShippingAddress!.Street).NotEmpty().When(c => c.ShippingAddress is not null);
        RuleFor(c => c.ShippingAddress!.Number).NotEmpty().When(c => c.ShippingAddress is not null);
        RuleFor(c => c.ShippingAddress!.Neighborhood).NotEmpty().When(c => c.ShippingAddress is not null);
        RuleFor(c => c.ShippingAddress!.City).NotEmpty().When(c => c.ShippingAddress is not null);
        RuleFor(c => c.ShippingAddress!.State).NotEmpty().Length(2).When(c => c.ShippingAddress is not null);
        RuleFor(c => c.CreatedBy).NotNull();
    }
}
