using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Exceptions;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;

namespace SkiRent.Desktop.ViewModels.Users
{
    public partial class UserListViewModel : BaseViewModel, IInitializeAsync
    {
        private readonly ISkiRentApi _skiRentApi = null!;
        private readonly CurrentUser _currentUser = null!;

        [ObservableProperty]
        private UserList _selectedUser = null!;

        public ObservableCollection<UserList> Users { get; } = [];

        public UserListViewModel()
        { }

        public UserListViewModel(ISkiRentApi skiRentApi)
        {
            _skiRentApi = skiRentApi;

            var user = Application.Current.Properties[nameof(CurrentUser)]
                ?? throw new CurrentUserNotFoundException("Current user not found in application properties.");
            _currentUser = (CurrentUser)user;
        }

        public async Task InitializeAsync()
        {
            var result = await _skiRentApi.Users.GetAllAsync();

            if (result.IsSuccessful)
            {
                Users.Clear();
                foreach (var user in result.Content)
                {
                    Users.Add(new()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Role = user.Role
                    });
                }
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            await InitializeAsync();
        }

        [RelayCommand]
        private async Task ShowUserEditAsync()
        {
            if (SelectedUser is not null)
            {
                await Navigator.Instance.NavigateToAsync<UserEditViewModel>(async vm =>
                    await vm.InitializeAsync(SelectedUser.Id));
            }
        }

        [RelayCommand]
        private async Task ShowUserCreateAsync()
        {
            await Navigator.Instance.NavigateToAsync<UserCreateViewModel>();
        }

        [RelayCommand]
        private async Task DeleteUserAsync()
        {
            if (SelectedUser is null)
            {
                return;
            }

            if (SelectedUser.Id == _currentUser.Id)
            {
                MessageBox.Show("Nem törölheted saját magadat.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show("Biztosan törölni szeretné ezt a felhasználót?", "Törlés megerősítése",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var deleteResult = await _skiRentApi.Users.DeleteAsync(SelectedUser.Id);

            if (deleteResult.IsSuccessful)
            {
                MessageBox.Show("A felhasználó sikeresen törölve lett.", "Sikeres törlés",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                await RefreshAsync();
                return;
            }

            MessageBox.Show("A felhasználót nem lehet törölni, mert aktív foglalások vannak hozzá rendelve.", "Hiba",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
