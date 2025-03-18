using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FluentValidation;

using SkiRent.Desktop.Services;
using SkiRent.Desktop.Utils;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.Desktop.ViewModels.Users
{
    public partial class UserCreateViewModel : BaseViewModel
    {
        private readonly ISkiRentApi _skiRentApi = null!;
        private readonly IValidator<CreateUserRequest> _validator = null!;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _isAdmin = true;

        public UserCreateViewModel()
        { }

        public UserCreateViewModel(ISkiRentApi skiRentApi, IValidator<CreateUserRequest> validator)
        {
            _skiRentApi = skiRentApi;
            _validator = validator;
        }

        [RelayCommand]
        private void GenerateNewPassword()
        {
            Password = PasswordGeneratorHelper.GeneratePassword();
        }

        [RelayCommand]
        private async Task CreateAsync()
        {
            var (_, request) = PrepareUserCreateRequest();

            await _validator.ValidateAndThrowAsync(request);

            var result = await _skiRentApi.Users.CreateAsync(request);

            if (result.IsSuccessful)
            {
                MessageBox.Show("A felhasználó sikeresen létrehozva.", "Sikeres létrehozás", MessageBoxButton.OK, MessageBoxImage.Information);

                if (IsAdmin)
                {
                    var updateResult = await _skiRentApi.Users.UpdateAsync(result.Content.Id, new()
                    {
                        Role = RoleTypes.Admin
                    });

                    if (!updateResult.IsSuccessful)
                    {
                        MessageBox.Show("A felhasználó létrejött, de az admin jogok beállítása sikertelen volt.",
                            "Részleges siker", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                await NavigateBackAsync();
                return;
            }

            MessageBox.Show("Hiba történt a felhasználó létrehozása során.", "Sikertelen mentés", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        [RelayCommand]
        private async Task BackAsync()
        {
            var (hasChanges, _) = PrepareUserCreateRequest();

            if (hasChanges && !ShowConfirmationDialog())
            {
                return;
            }

            await NavigateBackAsync();
        }

        private static async Task NavigateBackAsync()
        {
            await Navigator.Instance.NavigateToAsync<UserListViewModel>();
        }

        private static bool ShowConfirmationDialog()
        {
            var result = MessageBox.Show("Biztosan kilép mentés nélkül? Az adatok nem lesznek elmentve.", "Nem mentett adatok!",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        private (bool hasChanges, CreateUserRequest request) PrepareUserCreateRequest()
        {
            var hasChanges = false;

            if (!string.IsNullOrWhiteSpace(Email.Trim()))
            {
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(Password.Trim()))
            {
                hasChanges = true;
            }

            var request = new CreateUserRequest()
            {
                Email = Email,
                Password = Password
            };

            return (hasChanges, request);
        }
    }
}
