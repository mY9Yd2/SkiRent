﻿using System.IO.Abstractions;
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

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Memory;

using SkiRent.Desktop.Configurations;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.Utils;
using SkiRent.Desktop.ViewModels.Admin;
using SkiRent.Desktop.ViewModels.Bookings;
using SkiRent.Desktop.ViewModels.EquipmentCategories;
using SkiRent.Desktop.ViewModels.EquipmentImages;
using SkiRent.Desktop.ViewModels.Equipments;
using SkiRent.Desktop.ViewModels.Invoices;
using SkiRent.Desktop.ViewModels.Main;
using SkiRent.Desktop.ViewModels.Users;
using SkiRent.Desktop.Views.Admin;
using SkiRent.Desktop.Views.Bookings;
using SkiRent.Desktop.Views.EquipmentCategories;
using SkiRent.Desktop.Views.EquipmentImages;
using SkiRent.Desktop.Views.Equipments;
using SkiRent.Desktop.Views.Invoices;
using SkiRent.Desktop.Views.Main;
using SkiRent.Desktop.Views.Users;
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

        Configuration.Default.MemoryAllocator = MemoryAllocator.Create(new MemoryAllocatorOptions()
        {
            AllocationLimitMegabytes = 32
        });

        builder.Services.AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly(), Assembly.Load("SkiRent.Shared")]);

        builder.Services.AddSingleton<MainWindowViewModel>();
        builder.Services.AddSingleton<MainWindow>();

        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IUserService, UserService>();

        builder.Services.AddScoped<IMessageBoxService, MessageBoxService>();

        builder.Services.AddTransient<IFileSystem, FileSystem>();

        builder.Services.AddTransient<IProcessService, ProcessService>();

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

        builder.Services.AddTransient<EquipmentCreateViewModel>();
        builder.Services.AddTransient<EquipmentCreateView>();

        builder.Services.AddTransient<BookingListViewModel>();
        builder.Services.AddTransient<BookingListView>();

        builder.Services.AddTransient<BookingEditViewModel>();
        builder.Services.AddTransient<BookingEditView>();

        builder.Services.AddTransient<BookingItemListViewModel>();
        builder.Services.AddTransient<BookingItemListView>();

        builder.Services.AddTransient<EquipmentCategoryListViewModel>();
        builder.Services.AddTransient<EquipmentCategoryListView>();

        builder.Services.AddTransient<EquipmentCategoryEditViewModel>();
        builder.Services.AddTransient<EquipmentCategoryEditView>();

        builder.Services.AddTransient<EquipmentCategoryCreateViewModel>();
        builder.Services.AddTransient<EquipmentCategoryCreateView>();

        builder.Services.AddTransient<InvoiceListViewModel>();
        builder.Services.AddTransient<InvoiceListView>();

        builder.Services.AddTransient<UserListViewModel>();
        builder.Services.AddTransient<UserListView>();

        builder.Services.AddTransient<UserEditViewModel>();
        builder.Services.AddTransient<UserEditView>();

        builder.Services.AddTransient<UserCreateViewModel>();
        builder.Services.AddTransient<UserCreateView>();

        builder.Services.AddTransient<EquipmentImageListViewModel>();
        builder.Services.AddTransient<EquipmentImageListView>();

        builder.Services.AddTransient<EquipmentImageEditViewModel>();
        builder.Services.AddTransient<EquipmentImageEditView>();

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
