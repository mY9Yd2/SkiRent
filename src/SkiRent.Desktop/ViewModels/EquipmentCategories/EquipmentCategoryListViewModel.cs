using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;

namespace SkiRent.Desktop.ViewModels.EquipmentCategories
{
    public partial class EquipmentCategoryListViewModel : BaseViewModel, IInitializeAsync
    {
        private readonly ISkiRentApi _skiRentApi = null!;

        [ObservableProperty]
        private EquipmentCategoryList _selectedEquipmentCategory = null!;

        public ObservableCollection<EquipmentCategoryList> EquipmentCategories { get; } = [];

        public EquipmentCategoryListViewModel()
        { }

        public EquipmentCategoryListViewModel(ISkiRentApi skiRentApi)
        {
            _skiRentApi = skiRentApi;
        }

        public async Task InitializeAsync()
        {
            var result = await _skiRentApi.EquipmentCategories.GetAllAsync();

            if (result.IsSuccessful)
            {
                EquipmentCategories.Clear();
                foreach (var category in result.Content)
                {
                    EquipmentCategories.Add(new()
                    {
                        Id = category.Id,
                        Name = category.Name
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
        private async Task ShowEquipmentCategoryEditAsync()
        {
            if (SelectedEquipmentCategory is not null)
            {
                await Navigator.Instance.NavigateToAsync<EquipmentCategoryEditViewModel>(async vm =>
                    await vm.InitializeAsync(SelectedEquipmentCategory.Id, SelectedEquipmentCategory.Name));
            }
        }

        [RelayCommand]
        private async Task ShowEquipmentCategoryCreateAsync()
        {
            await Navigator.Instance.NavigateToAsync<EquipmentCategoryCreateViewModel>();
        }

        [RelayCommand]
        private async Task DeleteEquipmentCategoryAsync()
        {
            if (SelectedEquipmentCategory is null)
            {
                return;
            }

            var result = MessageBox.Show("Biztosan törölni szeretné ezt a kategóriát?", "Törlés megerősítése",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var deleteResult = await _skiRentApi.EquipmentCategories.DeleteAsync(SelectedEquipmentCategory.Id);

            if (deleteResult.IsSuccessful)
            {
                MessageBox.Show("A kategória sikeresen törölve lett.", "Sikeres törlés",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                await RefreshAsync();
                return;
            }

            MessageBox.Show("A kategóriát nem lehet törölni, mert még van hozzá kapcsolódó felszerelés.", "Hiba",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
