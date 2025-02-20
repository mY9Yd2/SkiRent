using System.Reflection;

using FluentValidation;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.ExceptionHandlers;
using SkiRent.Api.Services.Auth;
using SkiRent.Api.Services.EquipmentCategories;
using SkiRent.Api.Services.Equipments;
using SkiRent.Api.Services.Users;

namespace SkiRent.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var environment = builder.Environment;
        var configuration = builder.Configuration;

        services.Configure<SkiRentContextSettings>(options =>
                SkiRentContextSettingsConfiguration.Configure(options, environment, configuration));

        services.ConfigureAuthentication();
        services.ConfigureAuthorization();

        services.AddControllers()
            .AddJsonOptions(JsonOptionsConfiguration.Configure);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(SwaggerConfiguration.Configure);
        services.AddProblemDetails();

        services.AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly(), Assembly.Load("SkiRent.Shared")]);

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddDbContext<SkiRentContext>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IEquipmentCategoryService, EquipmentCategoryService>();
    }
}
