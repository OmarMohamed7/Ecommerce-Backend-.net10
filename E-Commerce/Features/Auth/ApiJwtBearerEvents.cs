using E_Commerce.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace E_Commerce.Features.Auth;

/// Returns 401/403 in the ApiResponse format instead of an empty body.
public class ApiJwtBearerEvents : JwtBearerEvents
{
    // 401: no token, invalid token or expired token
    public override Task Challenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse(); // stop the default empty 401

        var message = context.AuthenticateFailure is SecurityTokenExpiredException
            ? "Token has expired."
            : "Authentication required. Provide a valid token.";

        return Write(context.HttpContext, StatusCodes.Status401Unauthorized, message);
    }

    // 403: valid token, but the role/policy is not allowed
    public override Task Forbidden(ForbiddenContext context)
    {
        return Write(context.HttpContext, StatusCodes.Status403Forbidden,
            "You do not have permission to access this resource.");
    }

    private static Task Write(HttpContext http, int status, string message)
    {
        http.Response.StatusCode = status;
        return http.Response.WriteAsJsonAsync(ApiResponse.Fail(message));
    }
}