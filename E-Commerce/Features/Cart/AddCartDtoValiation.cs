using FluentValidation;

namespace E_Commerce.Features.Cart;

public class AddCartDtoValidation : AbstractValidator<AddToCartDto>
{
    public AddCartDtoValidation()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}