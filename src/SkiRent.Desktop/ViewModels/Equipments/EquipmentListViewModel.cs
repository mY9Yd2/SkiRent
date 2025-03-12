using System.Collections.ObjectModel;
using System.Globalization;

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
                    Equipments.Add(new()
                    {
                        Id = equipment.Id,
                        Name = equipment.Name,
                        CategoryId = equipment.CategoryId,
                        CategoryName = equipment.CategoryName,
                        PricePerDay = equipment.PricePerDay.ToString("C0", CultureInfo.CreateSpecificCulture("hu-HU")),
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
