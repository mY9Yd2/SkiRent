using SkiRent.Api.Data;

namespace SkiRent.Api.Configurations;

public static class SkiRentContextSettingsConfiguration
{
    public static void Configure(SkiRentContextSettings options, IHostEnvironment environment, IConfiguration configuration)
    {
        options.IsDevelopment = environment.IsDevelopment();
        options.ConnectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }
}
