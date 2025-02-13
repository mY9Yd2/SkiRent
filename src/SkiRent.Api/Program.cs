using Microsoft.EntityFrameworkCore;

using SkiRent.Api.Data;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.ExceptionHandlers;
using SkiRent.Api.Services.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SkiRentContextSettings>(options =>
{
    options.IsDevelopment = builder.Environment.IsDevelopment();
    options.ConnectionString = builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<SkiRentContext>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();
