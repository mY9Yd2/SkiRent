using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Admin;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Desktop.ViewModels.Main
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly ISkiRentApi _skiRentApi = null!;

        public MainViewModel()
        { }

        public MainViewModel(ISkiRentApi skiRentApi)
        {
            _skiRentApi = skiRentApi;
        }

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [RelayCommand]
        private async Task SignInAsync()
        {
            var signInResult = await _skiRentApi.Auth.SignInAsync(new()
            {
                Email = Email,
                Password = Password
            });

            if (!signInResult.IsSuccessful)
            {
                MessageBox.Show("Hibás email vagy jelszó.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = await _skiRentApi.Auth.MeAsync();

            if (result.IsSuccessful)
            {
                if (result.Content.Role != RoleTypes.Admin)
                {
                    MessageBox.Show("Az alkalmazás használatához adminisztrátori jogosultság szükséges.", "Hozzáférés megtagadva",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    await _skiRentApi.Auth.SignOutAsync(string.Empty);
                    return;
                }

                Application.Current.Properties[nameof(CurrentUser)] = new CurrentUser
                {
                    Id = result.Content.Id,
                    Email = result.Content.Email,
                    Role = result.Content.Role
                };

                Navigator.Instance.SwitchTo<AdminMainViewModel>();
            }
        }
    }
}
