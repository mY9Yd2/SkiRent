using Microsoft.AspNetCore.Authentication.Cookies;

namespace SkiRent.Api.Configurations;

public static class CookieAuthenticationOptionsConfiguration
{
    public static void Configure(CookieAuthenticationOptions options)
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
    }
}
