using System.Reflection;
using System.Text.Json.Serialization;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

using SkiRent.Api.Data;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.ExceptionHandlers;
using SkiRent.Api.Extensions;
using SkiRent.Api.Services.Auth;
using SkiRent.Api.Services.EquipmentCategories;
using SkiRent.Api.Services.Equipments;
using SkiRent.Api.Services.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SkiRentContextSettings>(options =>
{
    options.IsDevelopment = builder.Environment.IsDevelopment();
    options.ConnectionString = builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
});

builder.Services.ConfigureAuthentication();
builder.Services.ConfigureAuthorization();
builder.Services.ConfigurePolicies();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.AllowInputFormatterExceptionMessages = false;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly(), Assembly.Load("SkiRent.Shared")]);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<SkiRentContext>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IEquipmentCategoryService, EquipmentCategoryService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

await app.RunAsync();
