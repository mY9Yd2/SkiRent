
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;

namespace SkiRent.Desktop.ViewModels.Equipments
{
    public partial class EquipmentViewModel : BaseViewModel, IInitializeAsync
    {
        private readonly ISkiRentApi _skiRentApi = null!;

        public EquipmentViewModel()
        { }

        public EquipmentViewModel(ISkiRentApi skiRentApi)
        {
            _skiRentApi = skiRentApi;
        }

        public ObservableCollection<EquipmentList> Equipments { get; } = [];

        [ObservableProperty]
        private EquipmentList _selectedEquipment = null!;

        public async Task InitializeAsync()
        {
            var result = await _skiRentApi.Equipments.GetAllAsync();

            if (result.IsSuccessful)
            {
                Equipments.Clear();
                foreach (var equipment in result.Content)
                {
                    Equipments.Add(new()
                    {
                        Id = equipment.Id,
                        Name = equipment.Name,
                        CategoryId = equipment.CategoryId,
                        CategoryName = equipment.CategoryName,
                        PricePerDay = equipment.PricePerDay,
                        AvailableQuantity = equipment.AvailableQuantity
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
    }
}
