using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FluentValidation;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.Utils;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.Desktop.ViewModels.Users
{
    public partial class UserEditViewModel : BaseViewModel, IInitializeAsync<int>
    {
        private readonly ISkiRentApi _skiRentApi = null!;
        private readonly IValidator<UpdateUserRequest> _validator = null!;

        private UserList _originalUser = null!;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _selectedUserRole = string.Empty;

        public IEnumerable<string> UserRoles { get; } = UserRoleHelper.GetAllLocalizedUserRoles();

        public UserEditViewModel()
        { }

        public UserEditViewModel(ISkiRentApi skiRentApi, IValidator<UpdateUserRequest> validator)
        {
            _skiRentApi = skiRentApi;
            _validator = validator;
        }

        public async Task InitializeAsync(int userId)
        {
            var result = await _skiRentApi.Users.GetAsync(userId);

            if (result.IsSuccessful)
            {
                _originalUser = new()
                {
                    Id = result.Content.Id,
                    Email = result.Content.Email,
                    Role = result.Content.Role
                };

                Email = result.Content.Email;
                SelectedUserRole = UserRoleHelper.GetLocalizedString(result.Content.Role);
            }
        }

        [RelayCommand]
        private void GenerateNewPassword()
        {
            Password = PasswordGeneratorHelper.GeneratePassword();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            var (isModified, request) = PrepareUserUpdateRequest();

            if (!isModified)
            {
                await NavigateBackAsync();
                return;
            }

            await _validator.ValidateAndThrowAsync(request);

            var result = await _skiRentApi.Users.UpdateAsync(_originalUser.Id, request);

            if (result.IsSuccessful)
            {
                MessageBox.Show("A változtatások sikeresen elmentésre kerültek.", "Sikeres mentés", MessageBoxButton.OK, MessageBoxImage.Information);
                await NavigateBackAsync();
                return;
            }

            MessageBox.Show("Hiba történt a mentés során.", "Sikertelen mentés", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        [RelayCommand]
        private async Task BackAsync()
        {
            var (isModified, _) = PrepareUserUpdateRequest();

            if (isModified && !ShowConfirmationDialog())
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
            var result = MessageBox.Show("Biztosan kilép mentés nélkül?", "Nem mentett módosítások!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        private (bool isModified, UpdateUserRequest request) PrepareUserUpdateRequest()
        {
            var request = new UpdateUserRequest();
            var isModified = false;

            var userRole = UserRoleHelper.GetUserRoleFromLocalizedString(SelectedUserRole);

            if (Email.Trim() != _originalUser.Email
                && !string.IsNullOrWhiteSpace(Email.Trim()))
            {
                request = request with { Email = Email.Trim() };
                isModified = true;
            }

            if (!string.IsNullOrWhiteSpace(Password.Trim()))
            {
                request = request with { Password = Password.Trim() };
                isModified = true;
            }

            if (userRole != _originalUser.Role)
            {
                request = request with { Role = userRole };
                isModified = true;
            }

            return (isModified, request);
        }
    }
}
