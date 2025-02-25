using SkiRent.FakePay.Components;
using SkiRent.FakePay.Configurations;
using SkiRent.FakePay.Services.Payments;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.Configure<ClientOptions>(configuration.GetRequiredSection("Clients"));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();

builder.Services.AddFusionCache();

builder.Services.AddHttpClient();

builder.Services.AddScoped<PaymentService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    app.UseCors(builder =>
    {
        builder.SetIsOriginAllowed(_ => true);

        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
        builder.AllowCredentials();
    });
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

await app.RunAsync();
