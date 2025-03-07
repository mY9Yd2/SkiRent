using System.Net;
using System.Net.Http;
using System.Windows;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Refit;

using SkiRent.Desktop.Configurations;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Main;
using SkiRent.Desktop.Views.Main;
using SkiRent.Shared.Clients;

namespace SkiRent.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _app;

    public App()
    {
        var builder = Host.CreateApplicationBuilder();
        var configuration = builder.Configuration;

        builder.Services.Configure<AppSettings>(configuration.GetRequiredSection("AppSettings"));

        builder.Services.AddSingleton<CookieContainer>();

        using var serviceProvider = builder.Services.BuildServiceProvider();

        var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;

        builder.Services.AddRefitClient<ISkiRentApi>()
            .ConfigureHttpClient(client => client.BaseAddress = appSettings.BaseUrl)
            .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
            {
                var cookieContainer = serviceProvider.GetRequiredService<CookieContainer>();
                return new HttpClientHandler
                {
                    CookieContainer = cookieContainer
                };
            });

        builder.Services.AddSingleton<MainWindowViewModel>();
        builder.Services.AddSingleton<MainWindow>();

        builder.Services.AddSingleton<INavigationService, NavigationService>();

        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainView>();

        _app = builder.Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _app.StartAsync();

        Navigator.Initialize(_app.Services.GetRequiredService<INavigationService>());

        var mainWindow = _app.Services.GetRequiredService<MainWindow>();
        var navigationService = _app.Services.GetRequiredService<INavigationService>();

        mainWindow.Show();
        navigationService.NavigateTo<MainViewModel>();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _app.StopAsync();
        _app.Dispose();

        base.OnExit(e);
    }
}
