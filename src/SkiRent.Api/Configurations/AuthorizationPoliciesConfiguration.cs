using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using SkiRent.Api.Authorization.Requirements;
using SkiRent.Api.Data.Auth;

namespace SkiRent.Api.Configurations;

public static class AuthorizationPoliciesConfiguration
{
    public static void Configure(AuthorizationOptions options)
    {
        options.DefaultPolicy = BuildDefaultPolicy();
        options.AddPolicy(Policies.SelfOrAdminAccess, BuildSelfOrAdminAccessPolicy);
        options.AddPolicy(Policies.PaymentGatewayOnly, BuildPaymentGatewayOnlyPolicy);
    }

    private static AuthorizationPolicy BuildDefaultPolicy()
    {
        string[] authenticationSchemes = [
            CookieAuthenticationDefaults.AuthenticationScheme,
            BearerTokenDefaults.AuthenticationScheme
        ];

        var defaultPolicy = new AuthorizationPolicyBuilder(authenticationSchemes)
            .RequireAuthenticatedUser()
            .Build();

        return defaultPolicy;
    }

    private static void BuildPaymentGatewayOnlyPolicy(AuthorizationPolicyBuilder policy)
    {
        policy.AddRequirements(new PaymentGatewayOnlyRequirement());
    }

    private static void BuildSelfOrAdminAccessPolicy(AuthorizationPolicyBuilder policy)
    {
        policy.RequireAssertion(context =>
        {
            var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roleClaim = context.User.FindFirstValue(ClaimTypes.Role);

            if (roleClaim == Roles.Admin)
            {
                // Admins can view any user
                return true;
            }

            if (roleClaim == Roles.Customer && userIdClaim is not null)
            {
                if (context.Resource is not HttpContext httpContext)
                {
                    return false;
                }

                if (!httpContext.Request.RouteValues.TryGetValue("userId", out var routeUserIdObj) || routeUserIdObj is null)
                {
                    // No userId in route
                    return false;
                }

                if (!int.TryParse(routeUserIdObj.ToString(), out int requestedUserId))
                {
                    // Invalid userId format
                    return false;
                }

                return userIdClaim == requestedUserId.ToString();
            }

            return false;
        });
    }
}
