using SkiRent.Api.Configurations;
using SkiRent.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;

builder.ConfigureServices();

var app = builder.Build();

if (environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(CorsConfiguration.ConfigureDevelopmentCors);
}

app.UseAuthentication();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.MapGet("api", () => TypedResults.Ok());

await app.RunAsync();

public partial class Program
{
    protected Program()
    { }
}
