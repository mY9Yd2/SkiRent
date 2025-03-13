using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FluentValidation;

using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.EquipmentCategories;

namespace SkiRent.Desktop.ViewModels.EquipmentCategories
{
    public partial class EquipmentCategoryCreateViewModel : BaseViewModel
    {
        private readonly ISkiRentApi _skiRentApi = null!;
        private readonly IValidator<CreateEquipmentCategoryRequest> _validator = null!;

        [ObservableProperty]
        private string _name = string.Empty;

        public EquipmentCategoryCreateViewModel()
        { }

        public EquipmentCategoryCreateViewModel(ISkiRentApi skiRentApi, IValidator<CreateEquipmentCategoryRequest> validator)
        {
            _skiRentApi = skiRentApi;
            _validator = validator;
        }

        [RelayCommand]
        private async Task CreateAsync()
        {
            var (_, request) = PrepareEquipmentCategoryCreateRequest();

            await _validator.ValidateAndThrowAsync(request);

            var result = await _skiRentApi.EquipmentCategories.CreateAsync(request);

            if (result.IsSuccessful)
            {
                MessageBox.Show("Az új kategória sikeresen létrehozva.", "Sikeres létrehozás", MessageBoxButton.OK, MessageBoxImage.Information);
                await NavigateBackAsync();
                return;
            }

            MessageBox.Show("Hiba történt a kategória létrehozása során.", "Sikertelen mentés", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        [RelayCommand]
        private async Task BackAsync()
        {
            var (hasChanges, _) = PrepareEquipmentCategoryCreateRequest();

            if (hasChanges && !ShowConfirmationDialog())
            {
                return;
            }

            await NavigateBackAsync();
        }

        private static async Task NavigateBackAsync()
        {
            await Navigator.Instance.NavigateToAsync<EquipmentCategoryListViewModel>();
        }

        private static bool ShowConfirmationDialog()
        {
            var result = MessageBox.Show("Biztosan kilép mentés nélkül? Az adatok nem lesznek elmentve.", "Nem mentett adatok!",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        private (bool hasChanges, CreateEquipmentCategoryRequest request) PrepareEquipmentCategoryCreateRequest()
        {
            var hasChanges = false;

            if (!string.IsNullOrWhiteSpace(Name.Trim()))
            {
                hasChanges = true;
            }

            var request = new CreateEquipmentCategoryRequest()
            {
                Name = Name.Trim()
            };

            return (hasChanges, request);
        }
    }
}
