using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;

namespace SkiRent.Desktop.ViewModels.Equipments
{
    public partial class EquipmentListViewModel : BaseViewModel, IInitializeAsync
    {
        private readonly ISkiRentApi _skiRentApi = null!;

        [ObservableProperty]
        private EquipmentList _selectedEquipment = null!;

        public ObservableCollection<EquipmentList> Equipments { get; } = [];

        public EquipmentListViewModel()
        { }

        public EquipmentListViewModel(ISkiRentApi skiRentApi)
        {
            _skiRentApi = skiRentApi;
        }

        public async Task InitializeAsync()
        {
            var result = await _skiRentApi.Equipments.GetAllAsync();

            if (result.IsSuccessful)
            {
                Equipments.Clear();
                foreach (var equipment in result.Content)
                {
                    string imageId = equipment.MainImageId?.ToString() ?? "placeholder";

                    Equipments.Add(new()
                    {
                        Id = equipment.Id,
                        Name = equipment.Name,
                        CategoryId = equipment.CategoryId,
                        CategoryName = equipment.CategoryName,
                        PricePerDay = equipment.PricePerDay,
                        AvailableQuantity = equipment.AvailableQuantity,
                        ImageUrl = new Uri($"{_skiRentApi.Client.BaseAddress}images/{imageId}.jpg?t={DateTimeOffset.UtcNow.Ticks}")
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
        private async Task ShowEquipmentEditAsync()
        {
            if (SelectedEquipment is not null)
            {
                await Navigator.Instance.NavigateToAsync<EquipmentEditViewModel>(async vm =>
                    await vm.InitializeAsync(SelectedEquipment.Id));
            }
        }

        [RelayCommand]
        private async Task ShowEquipmentCreateAsync()
        {
            await Navigator.Instance.NavigateToAsync<EquipmentCreateViewModel>();
        }

        [RelayCommand]
        private async Task DeleteEquipmentAsync()
        {
            if (SelectedEquipment is null)
            {
                return;
            }

            var result = MessageBox.Show("Biztosan törölni szeretné ezt a felszerelést?", "Törlés megerősítése",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var deleteResult = await _skiRentApi.Equipments.DeleteAsync(SelectedEquipment.Id);

            if (deleteResult.IsSuccessful)
            {
                MessageBox.Show("A felszerelés sikeresen törölve lett.", "Sikeres törlés",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                await RefreshAsync();
                return;
            }

            MessageBox.Show("Hiba történt a felszerelés törlése során.", "Hiba",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
