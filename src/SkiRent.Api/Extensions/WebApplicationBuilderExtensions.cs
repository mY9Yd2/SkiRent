using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using SkiRent.Api.Data.Auth;

namespace SkiRent.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {
        string[] authenticationSchemes = [
            CookieAuthenticationDefaults.AuthenticationScheme
        ];

        var defaultPolicy = new AuthorizationPolicyBuilder(authenticationSchemes)
            .RequireAuthenticatedUser()
            .Build();

        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(defaultPolicy);

        return services;
    }

    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        services.ConfigureCookieAuthentication();

        return services;
    }

    public static IServiceCollection ConfigurePolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.SelfOrAdminAccess, policy => policy.RequireAssertion(context =>
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
                }));
        return services;
    }

    private static IServiceCollection ConfigureCookieAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "SkiRentAuth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.MaxAge = TimeSpan.FromDays(7);

                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    }
                };
            });
        return services;
    }
}
