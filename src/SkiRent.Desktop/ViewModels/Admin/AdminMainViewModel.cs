using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Exceptions;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Desktop.ViewModels.Main;
using SkiRent.Shared.Clients;

namespace SkiRent.Desktop.ViewModels.Admin
{
    public partial class AdminMainViewModel : BaseViewModel, IViewUpdater
    {
        private readonly ISkiRentApi _skiRentApi = null!;

        public AdminMainViewModel()
        { }

        public AdminMainViewModel(ISkiRentApi skiRentApi, AdminMenuViewModel adminMenuViewModel)
        {
            _skiRentApi = skiRentApi;
            _adminMenuViewModel = adminMenuViewModel;

            var user = Application.Current.Properties[nameof(CurrentUser)]
                ?? throw new CurrentUserNotFoundException("Current user not found in application properties.");
            CurrentUser = (CurrentUser)user;
        }

        [ObservableProperty]
        private CurrentUser _currentUser = null!;

        [ObservableProperty]
        private AdminMenuViewModel _adminMenuViewModel = null!;

        [ObservableProperty]
        private BaseViewModel _currentView = null!;

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
            var signOutResult = await _skiRentApi.Auth.SignOutAsync(string.Empty);

            if (signOutResult.IsSuccessful)
            {
                Application.Current.Properties[nameof(CurrentUser)] = null;
                Navigator.Instance.SwitchTo<MainWindowViewModel>();
                await Navigator.Instance.NavigateToAsync<MainViewModel>();
            }
        }
    }
}
