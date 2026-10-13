using System.Text.Json.Serialization;

namespace E_Commerce.Common;

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

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

// Non-generic helpers for responses that carry no data (errors from middleware, etc.)
public static class ApiResponse
{
    public static ApiResponse<object> Fail(string message)
    {
        return ApiResponse<object>.Fail(message);
    }

    public static ApiResponse<object> Fail(params ApiError[] errors)
    {
        return ApiResponse<object>.Fail(errors);
    }
}

public sealed record ApiError(string? Field, string Message);