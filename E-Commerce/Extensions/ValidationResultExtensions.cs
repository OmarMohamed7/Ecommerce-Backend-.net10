using E_Commerce.DTOs;
using FluentValidation.Results;

namespace E_Commerce.Extensions;

public static class ValidationResultExtensions
{
    public static ApiError[] ToApiErrors(this ValidationResult result)
    {
        return result.Errors
            .Select(e => new ApiError(e.PropertyName, e.ErrorMessage))
            .ToArray();
    }
}