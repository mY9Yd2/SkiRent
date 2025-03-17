using System.IO.Abstractions;
using System.Reflection;

using FluentValidation;

using Microsoft.AspNetCore.Authorization;

using QuestPDF.Infrastructure;

using SkiRent.Api.Authorization.Handlers;
using SkiRent.Api.Configurations;
using SkiRent.Api.Data;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.ExceptionHandlers;
using SkiRent.Api.Services.Auth;
using SkiRent.Api.Services.Bookings;
using SkiRent.Api.Services.EquipmentCategories;
using SkiRent.Api.Services.EquipmentImages;
using SkiRent.Api.Services.Equipments;
using SkiRent.Api.Services.Invoices;
using SkiRent.Api.Services.Payments;
using SkiRent.Api.Services.Users;

using ZiggyCreatures.Caching.Fusion;

namespace SkiRent.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var environment = builder.Environment;
        var configuration = builder.Configuration;

        services.Configure<AppSettings>(configuration.GetRequiredSection("AppSettings"));
        services.Configure<PaymentGatewayOptions>(configuration.GetRequiredSection("PaymentGateway"));
        services.Configure<SkiRentContextSettings>(options =>
                SkiRentContextSettingsConfiguration.Configure(options, environment, configuration));

        QuestPDF.Settings.License = LicenseType.Community;

        services.ConfigureAuthentication();
        services.ConfigureAuthorization();

        services.ConfigureImageSharp();

        services.AddControllers()
            .AddJsonOptions(JsonOptionsConfiguration.Configure);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(SwaggerConfiguration.Configure);
        services.AddProblemDetails();

        services.AddHttpClient();

        services.AddFusionCache("SkiRent.Api.Cache")
            .AsKeyedServiceByCacheName();

        services.AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly(), Assembly.Load("SkiRent.Shared")]);

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddDbContext<SkiRentContext>();

        services.AddSingleton<IAuthorizationHandler, PaymentGatewayOnlyHandler>();
        services.AddSingleton<IAuthorizationHandler, SelfOrAdminAccessHandler>();
        services.AddSingleton<IAuthorizationHandler, CustomerOrAdminAccessHandler>();

        services.AddTransient<IFileSystem, FileSystem>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IEquipmentCategoryService, EquipmentCategoryService>();
        services.AddScoped<IEquipmentImageService, EquipmentImageService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
    }
}
