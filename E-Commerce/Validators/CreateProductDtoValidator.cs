using E_Commerce.DTOs;
using FluentValidation;

namespace E_Commerce.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(product => product.Name)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(50)
            .WithMessage("Product name cannot exceed 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(150)
            .WithMessage("Description cannot exceed 150 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock cannot be negative.");
    }
}