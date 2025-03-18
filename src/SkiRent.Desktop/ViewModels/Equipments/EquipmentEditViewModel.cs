using System.Collections.ObjectModel;
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
        private readonly IValidator<UpdateEquipmentRequest> _validator = null!;

        private GetEquipmentResponse _originalEquipment = null!;
        private Guid? _currentImageId = null;

        [ObservableProperty]
        private bool _isImageSelectorVisible = false;

        [ObservableProperty]
        private EquipmentImagesList? _selectedImage;

        [ObservableProperty]
        private Uri _mainImageUrl = null!;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private int _pricePerDay;

        [ObservableProperty]
        private int _availableQuantity;

        [ObservableProperty]
        private EquipmentCategory _selectedEquipmentCategory = null!;

        public ObservableCollection<EquipmentCategory> EquipmentCategories { get; } = [];

        public ObservableCollection<EquipmentImagesList> AvailableImages { get; } = [];

        public EquipmentEditViewModel()
        { }

        public EquipmentEditViewModel(ISkiRentApi skiRentApi, IValidator<UpdateEquipmentRequest> validator)
        {
            _skiRentApi = skiRentApi;
            _validator = validator;
            ClearMainImage();
        }

        public async Task InitializeAsync(int equipmentId)
        {
            var result = await _skiRentApi.Equipments.GetAsync(equipmentId);
            var categoriesResult = await _skiRentApi.EquipmentCategories.GetAllAsync();

            if (result.IsSuccessful && categoriesResult.IsSuccessful)
            {
                Name = result.Content.Name;
                Description = result.Content.Description ?? string.Empty;
                PricePerDay = (int)result.Content.PricePerDay;
                AvailableQuantity = result.Content.AvailableQuantity;

                _currentImageId = result.Content.MainImageId;
                if (result.Content.MainImageId is not null)
                {
                    MainImageUrl = new Uri($"{_skiRentApi.Client.BaseAddress}images/{result.Content.MainImageId}.jpg?t={DateTimeOffset.UtcNow.Ticks}");
                }

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

        [RelayCommand]
        private async Task ToggleImageSelectionAsync()
        {
            IsImageSelectorVisible = !IsImageSelectorVisible;
            if (IsImageSelectorVisible)
            {
                await LoadAvailableImagesAsync();
            }
        }

        [RelayCommand]
        private async Task SelectMainImageAsync()
        {
            if (SelectedImage is not null)
            {
                MainImageUrl = SelectedImage.ImageUrl;
                _currentImageId = SelectedImage.Id;
                await ToggleImageSelectionAsync();
            }
        }

        [RelayCommand]
        private void ClearMainImage()
        {
            MainImageUrl = new Uri($"{_skiRentApi.Client.BaseAddress}images/placeholder.jpg");
            SelectedImage = null;
            _currentImageId = null;
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

            if (Name.Trim() != _originalEquipment.Name
                && !string.IsNullOrWhiteSpace(Name.Trim()))
            {
                request = request with { Name = Name.Trim() };
                isModified = true;
            }

            if (Description.Trim() != (_originalEquipment.Description ?? string.Empty))
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

            if (_currentImageId != _originalEquipment.MainImageId)
            {
                request = request with { MainImageId = _currentImageId };
                isModified = true;
            }

            return (isModified, request);
        }

        private async Task LoadAvailableImagesAsync()
        {
            var result = await _skiRentApi.EquipmentImages.GetAllAsync();

            if (result.IsSuccessful)
            {
                AvailableImages.Clear();
                foreach (var image in result.Content)
                {
                    AvailableImages.Add(new()
                    {
                        Id = image.Id,
                        DisplayName = image.DisplayName,
                        CreatedAt = image.CreatedAt,
                        ImageUrl = new Uri($"{_skiRentApi.Client.BaseAddress}images/{image.Id}.jpg?t={DateTimeOffset.UtcNow.Ticks}")
                    });
                }
            }
        }
    }
}
