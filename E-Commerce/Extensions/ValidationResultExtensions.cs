using FluentValidation.Results;
using E_Commerce.Common;

namespace E_Commerce.Extensions;

public static class ValidationResultExtensions
{
    public static ApiError[] ToApiErrors(this ValidationResult result)
    {
        return [.. result.Errors.Select(e => new ApiError(e.PropertyName, e.ErrorMessage))];
    }
}