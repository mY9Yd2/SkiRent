using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Windows;

using FluentValidation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Refit;

using SkiRent.Desktop.Configurations;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Admin;
using SkiRent.Desktop.ViewModels.Bookings;
using SkiRent.Desktop.ViewModels.Equipments;
using SkiRent.Desktop.ViewModels.Main;
using SkiRent.Desktop.Views.Admin;
using SkiRent.Desktop.Views.Bookings;
using SkiRent.Desktop.Views.Equipments;
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

        builder.Services.AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly(), Assembly.Load("SkiRent.Shared")]);

        builder.Services.AddSingleton<MainWindowViewModel>();
        builder.Services.AddSingleton<MainWindow>();

        builder.Services.AddSingleton<INavigationService, NavigationService>();

        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainView>();

        builder.Services.AddSingleton<AdminMainViewModel>();
        builder.Services.AddSingleton<AdminMainView>();

        builder.Services.AddSingleton<AdminMenuViewModel>();
        builder.Services.AddSingleton<AdminMenu>();

        builder.Services.AddTransient<EquipmentListViewModel>();
        builder.Services.AddTransient<EquipmentListView>();

        builder.Services.AddTransient<EquipmentEditViewModel>();
        builder.Services.AddTransient<EquipmentEditView>();

        builder.Services.AddTransient<BookingListViewModel>();
        builder.Services.AddTransient<BookingListView>();

        builder.Services.AddTransient<BookingEditViewModel>();
        builder.Services.AddTransient<BookingEditView>();

        _app = builder.Build();

        Current.DispatcherUnhandledException += (sender, args) =>
        {
            if (args.Exception is ValidationException)
            {
                MessageBox.Show("Érvénytelen adatok!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show($"Unhandled exception:\n{args.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            args.Handled = true;
        };
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _app.StartAsync();

        Navigator.Initialize(_app.Services.GetRequiredService<INavigationService>());

        var mainWindow = _app.Services.GetRequiredService<MainWindow>();
        var navigationService = _app.Services.GetRequiredService<INavigationService>();

        mainWindow.Show();
        await navigationService.NavigateToAsync<MainViewModel>();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _app.StopAsync();
        _app.Dispose();

        base.OnExit(e);
    }
}
