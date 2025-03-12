using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FluentValidation;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Equipments;

namespace SkiRent.Desktop.ViewModels.Equipments
{
    public partial class EquipmentEditViewModel : BaseViewModel, IInitializeAsync<int>
    {
        private readonly ISkiRentApi _skiRentApi = null!;

        public EquipmentEditViewModel()
        { }

        public EquipmentEditViewModel(ISkiRentApi skiRentApi, IValidator<UpdateEquipmentRequest> validator)
        {
            _skiRentApi = skiRentApi;
            _validator = validator;
        }

        public async Task InitializeAsync(int equipmentId)
        {
            var result = await _skiRentApi.Equipments.GetAsync(equipmentId);
            var categoriesResult = await _skiRentApi.EquipmentCategories.GetAllAsync();

            if (result.IsSuccessful && categoriesResult.IsSuccessful)
            {
                Name = result.Content.Name;
                Description = result.Content.Description ?? string.Empty;
                PricePerDay = result.Content.PricePerDay;
                AvailableQuantity = result.Content.AvailableQuantity;

                EquipmentCategories.Clear();
                foreach (var equipmentCategory in categoriesResult.Content)
                {
                    EquipmentCategories.Add(new()
                    {
                        Id = equipmentCategory.Id,
                        Name = equipmentCategory.Name
                    });
                }

                SelectedEquipmentCategory = EquipmentCategories.First(category => category.Id == result.Content.CategoryId);
                _originalEquipment = result.Content;
            }
        }

        private GetEquipmentResponse _originalEquipment = null!;

        private readonly IValidator<UpdateEquipmentRequest> _validator = null!;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        private decimal _pricePerDay;

        public decimal PricePerDay
        {
            get => _pricePerDay;
            set
            {
                SetProperty(ref _pricePerDay, value);
                PricePerDayFormatted = value.ToString("C0", CultureInfo.CreateSpecificCulture("hu-HU"));
            }
        }

        [ObservableProperty]
        private string _pricePerDayFormatted = string.Empty;

        [ObservableProperty]
        private int _availableQuantity;

        [ObservableProperty]
        private EquipmentCategory _selectedEquipmentCategory = null!;

        public ObservableCollection<EquipmentCategory> EquipmentCategories { get; } = [];

        [RelayCommand]
        private async Task SaveAsync()
        {
            var (isModified, request) = PrepareEquipmentUpdateRequest();

            if (!isModified)
            {
                await NavigateBackAsync();
                return;
            }

            await _validator.ValidateAndThrowAsync(request);

            var result = await _skiRentApi.Equipments.UpdateAsync(_originalEquipment.Id, request);

            if (result.IsSuccessful)
            {
                MessageBox.Show("A változtatások sikeresen elmentésre kerültek.", "Sikeres mentés", MessageBoxButton.OK, MessageBoxImage.Information);
                await NavigateBackAsync();
                return;
            }

            MessageBox.Show("Hiba történt a mentés során.", "Sikertelen mentés", MessageBoxButton.OK, MessageBoxImage.Error);
            await InitializeAsync(_originalEquipment.Id);
        }

        [RelayCommand]
        private async Task BackAsync()
        {
            var (isModified, _) = PrepareEquipmentUpdateRequest();

            if (isModified && !ShowConfirmationDialog())
            {
                return;
            }

            await NavigateBackAsync();
        }

        private static async Task NavigateBackAsync()
        {
            await Navigator.Instance.NavigateToAsync<EquipmentListViewModel>();
        }

        private static bool ShowConfirmationDialog()
        {
            var result = MessageBox.Show("Biztosan kilép mentés nélkül?", "Nem mentett módosítások!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        private (bool isModified, UpdateEquipmentRequest request) PrepareEquipmentUpdateRequest()
        {
            var request = new UpdateEquipmentRequest();
            var isModified = false;

            if (Name != _originalEquipment.Name
                && !string.IsNullOrWhiteSpace(Name.Trim()))
            {
                request = request with { Name = Name.Trim() };
                isModified = true;
            }

            if (Description != _originalEquipment.Description
                && !string.IsNullOrWhiteSpace(Description.Trim()))
            {
                request = request with { Description = Description.Trim() };
                isModified = true;
            }

            if (PricePerDay != _originalEquipment.PricePerDay)
            {
                request = request with { PricePerDay = PricePerDay };
                isModified = true;
            }

            if (AvailableQuantity != _originalEquipment.AvailableQuantity)
            {
                request = request with { AvailableQuantity = AvailableQuantity };
                isModified = true;
            }

            if (SelectedEquipmentCategory.Id != _originalEquipment.CategoryId)
            {
                request = request with { CategoryId = SelectedEquipmentCategory.Id };
                isModified = true;
            }

            return (isModified, request);
        }
    }
}
