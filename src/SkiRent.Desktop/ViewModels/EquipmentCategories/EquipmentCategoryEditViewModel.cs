using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FluentValidation;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.EquipmentCategories;

namespace SkiRent.Desktop.ViewModels.EquipmentCategories
{
    public partial class EquipmentCategoryEditViewModel : BaseViewModel, IInitializeAsync<int, string>
    {
        private readonly ISkiRentApi _skiRentApi = null!;
        private readonly IValidator<UpdateEquipmentCategoryRequest> _validator = null!;

        private EquipmentCategory _originalEquipmentCategory = null!;

        [ObservableProperty]
        private string _name = string.Empty;

        public EquipmentCategoryEditViewModel()
        { }

        public EquipmentCategoryEditViewModel(ISkiRentApi skiRentApi, IValidator<UpdateEquipmentCategoryRequest> validator)
        {
            _skiRentApi = skiRentApi;
            _validator = validator;
        }

        public Task InitializeAsync(int equipmentCategoryId, string name)
        {
            _originalEquipmentCategory = new()
            {
                Id = equipmentCategoryId,
                Name = name
            };

            Name = name;

            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            var (isModified, request) = PrepareEquipmentCategoryUpdateRequest();

            if (!isModified)
            {
                await NavigateBackAsync();
                return;
            }

            await _validator.ValidateAndThrowAsync(request);

            var result = await _skiRentApi.EquipmentCategories.UpdateAsync(_originalEquipmentCategory.Id, request);

            if (result.IsSuccessful)
            {
                MessageBox.Show("A változtatások sikeresen elmentésre kerültek.", "Sikeres mentés", MessageBoxButton.OK, MessageBoxImage.Information);
                await NavigateBackAsync();
                return;
            }

            MessageBox.Show("Hiba történt a mentés során.", "Sikertelen mentés", MessageBoxButton.OK, MessageBoxImage.Error);
            await InitializeAsync(_originalEquipmentCategory.Id, _originalEquipmentCategory.Name);
        }

        [RelayCommand]
        private async Task BackAsync()
        {
            var (isModified, _) = PrepareEquipmentCategoryUpdateRequest();

            if (isModified && !ShowConfirmationDialog())
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
            var result = MessageBox.Show("Biztosan kilép mentés nélkül?", "Nem mentett módosítások!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        private (bool isModified, UpdateEquipmentCategoryRequest request) PrepareEquipmentCategoryUpdateRequest()
        {
            var request = new UpdateEquipmentCategoryRequest();
            var isModified = false;

            if (Name != _originalEquipmentCategory.Name
                && !string.IsNullOrWhiteSpace(Name.Trim()))
            {
                request = request with { Name = Name.Trim() };
                isModified = true;
            }

            return (isModified, request);
        }
    }
}
