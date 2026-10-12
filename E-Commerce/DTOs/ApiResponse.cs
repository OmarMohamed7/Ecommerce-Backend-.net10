using System.Text.Json.Serialization;

namespace E_Commerce.DTOs;

public sealed record ApiResponse<T>(
    bool Success,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    T? Data,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyList<ApiError>? Errors)
{
    public static ApiResponse<T> Ok(T data)
    {
        return new ApiResponse<T>(true, data, null);
    }

    public static ApiResponse<T> Fail(params ApiError[] errors)
    {
        return new ApiResponse<T>(false, default, errors);
    }

    public static ApiResponse<T> Fail(string message)
    {
        return Fail(new ApiError(null, message));
    }
}

public sealed record ApiError(string? Field, string Message);