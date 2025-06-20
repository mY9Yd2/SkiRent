﻿using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Exceptions;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.Utils;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Desktop.ViewModels.Main;
using SkiRent.Shared.Clients;

namespace SkiRent.Desktop.ViewModels.Admin
{
    public partial class AdminMainViewModel : BaseViewModel, IViewUpdater
    {
        private readonly ISkiRentApi _skiRentApi = null!;
        private readonly IUserService _userService = null!;

        [ObservableProperty]
        private CurrentUser _currentUser = null!;

        [ObservableProperty]
        private AdminMenuViewModel _adminMenuViewModel = null!;

        [ObservableProperty]
        private BaseViewModel _currentView = null!;

        public AdminMainViewModel()
        { }

        public AdminMainViewModel(ISkiRentApi skiRentApi, AdminMenuViewModel adminMenuViewModel, IUserService userService)
        {
            _skiRentApi = skiRentApi;
            _adminMenuViewModel = adminMenuViewModel;
            _userService = userService;

            CurrentUser = userService.CurrentUser
                ?? throw new CurrentUserNotFoundException("Current user not found in application properties.");
        }

        public void UpdateCurrentView(BaseViewModel viewModel)
        {
            if (CurrentView is null || CurrentView.GetType().Name != viewModel.GetType().Name)
            {
                CurrentView = viewModel;
            }
        }

        [RelayCommand]
        private async Task SignOutAsync()
        {
            var result = MessageBox.Show("Biztosan ki szeretne jelentkezni?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var signOutResult = await _skiRentApi.Auth.SignOutAsync(string.Empty);

            if (signOutResult.IsSuccessful)
            {
                _userService.CurrentUser = null;
                Navigator.Instance.SwitchTo<MainWindowViewModel>();
                await Navigator.Instance.NavigateToAsync<MainViewModel>();
            }
        }
    }
}
