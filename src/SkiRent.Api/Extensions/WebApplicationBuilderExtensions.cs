using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

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

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = defaultPolicy;
        });

        return services;
    }

    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        services.ConfigureCookieAuthentication();

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
            });
        return services;
    }
}
