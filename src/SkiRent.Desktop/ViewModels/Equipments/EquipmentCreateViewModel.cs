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
    public partial class EquipmentCreateViewModel : BaseViewModel, IInitializeAsync
    {
        private readonly ISkiRentApi _skiRentApi = null!;
        private readonly IValidator<CreateEquipmentRequest> _validator = null!;

        private Guid? _selectedImageId = null;

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
        private int? _pricePerDay = null;

        [ObservableProperty]
        private int? _availableQuantity = null;

        [ObservableProperty]
        private EquipmentCategory _selectedEquipmentCategory = null!;

        public ObservableCollection<EquipmentCategory> EquipmentCategories { get; } = [];

        public ObservableCollection<EquipmentImagesList> AvailableImages { get; } = [];

        public EquipmentCreateViewModel()
        { }

        public EquipmentCreateViewModel(ISkiRentApi skiRentApi, IValidator<CreateEquipmentRequest> validator)
        {
            _skiRentApi = skiRentApi;
            _validator = validator;
            ClearMainImage();
        }

        public async Task InitializeAsync()
        {
            var categoriesResult = await _skiRentApi.EquipmentCategories.GetAllAsync();

            if (categoriesResult.IsSuccessful)
            {
                EquipmentCategories.Clear();
                EquipmentCategories.Add(new()
                {
                    Id = -1,
                    Name = string.Empty
                });
                SelectedEquipmentCategory = EquipmentCategories[0];

                foreach (var equipmentCategory in categoriesResult.Content)
                {
                    EquipmentCategories.Add(new()
                    {
                        Id = equipmentCategory.Id,
                        Name = equipmentCategory.Name
                    });
                }
            }
        }

        [RelayCommand]
        private async Task CreateAsync()
        {
            var (_, request) = PrepareEquipmentCreateRequest();

            await _validator.ValidateAndThrowAsync(request);

            var result = await _skiRentApi.Equipments.CreateAsync(request);

            if (result.IsSuccessful)
            {
                MessageBox.Show("Az új felszerelés sikeresen létrehozva.", "Sikeres létrehozás", MessageBoxButton.OK, MessageBoxImage.Information);
                await NavigateBackAsync();
                return;
            }

            MessageBox.Show("Hiba történt a felszerelés létrehozása során.", "Sikertelen mentés", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        [RelayCommand]
        private async Task BackAsync()
        {
            var (hasChanges, _) = PrepareEquipmentCreateRequest();

            if (hasChanges && !ShowConfirmationDialog())
            {
                return;
            }

            await NavigateBackAsync();
        }

        [RelayCommand]
        private async Task ToggleImageSelectionAsync()
        {
            IsImageSelectorVisible = !IsImageSelectorVisible;
            SelectedImage = null;
            if (IsImageSelectorVisible)
            {
                await LoadAvailableImagesAsync();
            }
        }

        [RelayCommand]
        private async Task SelectMainImageAsync()
        {
            if (SelectedImage is null)
            {
                return;
            }

            MainImageUrl = SelectedImage.ImageUrl;
            _selectedImageId = SelectedImage.Id;
            await ToggleImageSelectionAsync();
        }

        [RelayCommand]
        private void ClearMainImage()
        {
            MainImageUrl = new Uri($"{_skiRentApi.Client.BaseAddress}images/placeholder.jpg");
            SelectedImage = null;
            _selectedImageId = null;
        }

        private static async Task NavigateBackAsync()
        {
            await Navigator.Instance.NavigateToAsync<EquipmentListViewModel>();
        }

        private static bool ShowConfirmationDialog()
        {
            var result = MessageBox.Show("Biztosan kilép mentés nélkül? Az adatok nem lesznek elmentve.", "Nem mentett adatok!",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        private (bool hasChanges, CreateEquipmentRequest request) PrepareEquipmentCreateRequest()
        {
            var hasChanges = false;

            if (!string.IsNullOrWhiteSpace(Name.Trim()))
            {
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(Description.Trim()))
            {
                hasChanges = true;
            }

            if (PricePerDay is not null)
            {
                hasChanges = true;
            }

            if (AvailableQuantity is not null)
            {
                hasChanges = true;
            }

            if (SelectedEquipmentCategory.Id != -1)
            {
                hasChanges = true;
            }

            if (!MainImageUrl.AbsolutePath.EndsWith("placeholder.jpg"))
            {
                hasChanges = true;
            }

            var request = new CreateEquipmentRequest()
            {
                Name = Name.Trim(),
                Description = string.IsNullOrWhiteSpace(Description.Trim()) ? null : Description.Trim(),
                CategoryId = SelectedEquipmentCategory.Id,
                PricePerDay = PricePerDay ?? -1,
                AvailableQuantity = AvailableQuantity ?? -1,
                MainImageId = _selectedImageId
            };

            return (hasChanges, request);
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
                        ImageUrl = new Uri($"{_skiRentApi.Client.BaseAddress}images/{image.Id}.jpg")
                    });
                }
            }
        }
    }
}
