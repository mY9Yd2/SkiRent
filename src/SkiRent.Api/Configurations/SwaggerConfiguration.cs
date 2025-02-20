using Swashbuckle.AspNetCore.SwaggerGen;

namespace SkiRent.Api.Configurations;

public static class SwaggerConfiguration
{
    public static void Configure(SwaggerGenOptions options)
    {
        options.SupportNonNullableReferenceTypes();
    }
}
