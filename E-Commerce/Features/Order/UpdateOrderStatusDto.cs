using FluentValidation;

namespace E_Commerce.Features.Order;

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}

public class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDto>
{
    public UpdateOrderStatusDtoValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}