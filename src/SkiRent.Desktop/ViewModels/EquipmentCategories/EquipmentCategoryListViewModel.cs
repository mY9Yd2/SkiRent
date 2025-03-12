using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Models;
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
        private Task ShowEquipmentCategoryEditAsync()
        {
            throw new NotImplementedException();
        }
    }
}
