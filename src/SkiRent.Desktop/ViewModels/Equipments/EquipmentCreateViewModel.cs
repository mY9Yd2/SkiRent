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

        public EquipmentCreateViewModel()
        { }

        public EquipmentCreateViewModel(ISkiRentApi skiRentApi, IValidator<CreateEquipmentRequest> validator)
        {
            _skiRentApi = skiRentApi;
            _validator = validator;
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

            var request = new CreateEquipmentRequest()
            {
                Name = Name.Trim(),
                Description = Description.Trim(),
                CategoryId = SelectedEquipmentCategory.Id,
                PricePerDay = PricePerDay ?? -1,
                AvailableQuantity = AvailableQuantity ?? -1
            };

            return (hasChanges, request);
        }
    }
}
