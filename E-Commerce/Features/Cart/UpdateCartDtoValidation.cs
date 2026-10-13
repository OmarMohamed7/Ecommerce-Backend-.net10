using FluentValidation;

namespace E_Commerce.Features.Cart;

public class UpdateCartItemDtoValidation : AbstractValidator<UpdateCartItemDto>
{
    public UpdateCartItemDtoValidation()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}