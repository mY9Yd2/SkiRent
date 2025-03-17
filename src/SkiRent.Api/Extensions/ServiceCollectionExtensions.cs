using Microsoft.AspNetCore.Authentication.Cookies;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Memory;

using SkiRent.Api.Configurations;

namespace SkiRent.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(AuthorizationPoliciesConfiguration.Configure);
        return services;
    }

    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationOptionsConfiguration.Configure)
            .AddBearerToken();
        return services;
    }

    public static IServiceCollection ConfigureImageSharp(this IServiceCollection services)
    {
        Configuration.Default.MemoryAllocator = MemoryAllocator.Create(new MemoryAllocatorOptions()
        {
            AllocationLimitMegabytes = 32
        });
        return services;
    }
}
