using Microsoft.AspNetCore.Cors.Infrastructure;

namespace SkiRent.Api.Configurations;

public static class CorsConfiguration
{
    public static void ConfigureDevelopmentCors(CorsPolicyBuilder builder)
    {
        builder.SetIsOriginAllowed(_ => true);

        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
        builder.AllowCredentials();
    }
}
