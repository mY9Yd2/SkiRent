using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;

namespace SkiRent.Desktop.ViewModels.Users
{
    public partial class UserListViewModel : BaseViewModel, IInitializeAsync
    {
        private readonly ISkiRentApi _skiRentApi = null!;

        [ObservableProperty]
        private UserList _selectedUser = null!;

        public ObservableCollection<UserList> Users { get; } = [];

        public UserListViewModel()
        { }

        public UserListViewModel(ISkiRentApi skiRentApi)
        {
            _skiRentApi = skiRentApi;
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
        private Task ShowUserEditAsync()
        {
            throw new NotImplementedException();
        }
    }
}
