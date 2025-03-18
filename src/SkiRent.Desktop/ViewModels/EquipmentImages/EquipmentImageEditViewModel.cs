
using System.IO;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FluentValidation;

using Microsoft.Win32;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.EquipmentImages;

namespace SkiRent.Desktop.ViewModels.EquipmentImages
{
    public partial class EquipmentImageEditViewModel : BaseViewModel, IInitializeAsync<Guid, string, Uri>
    {
        private readonly ISkiRentApi _skiRentApi = null!;
        private readonly IValidator<UpdateEquipmentImageRequest> _validator = null!;

        private Guid _imageId = Guid.Empty;
        private string? _originalDisplayName = null;
        private Uri _originalImageUrl = null!;

        [ObservableProperty]
        private Uri _mainImageUrl = null!;

        [ObservableProperty]
        private string _displayName = string.Empty;

        public EquipmentImageEditViewModel()
        { }

        public EquipmentImageEditViewModel(ISkiRentApi skiRentApi, IValidator<UpdateEquipmentImageRequest> validator)
        {
            _skiRentApi = skiRentApi;
            _validator = validator;
        }

        public Task InitializeAsync(Guid imageId, string? displayName, Uri imageUrl)
        {
            _imageId = imageId;

            DisplayName = displayName ?? string.Empty;
            _originalDisplayName = displayName;

            MainImageUrl = new UriBuilder(imageUrl) { Query = $"t={DateTime.UtcNow.Ticks}" }.Uri;
            _originalImageUrl = new UriBuilder(imageUrl) { Query = $"t={DateTime.UtcNow.Ticks}" }.Uri;

            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            var (isModified, request) = PrepareEquipmentImageUpdateRequest();

            if (!isModified)
            {
                await NavigateBackAsync();
                return;
            }

            await _validator.ValidateAndThrowAsync(request);

            var result = await _skiRentApi.EquipmentImages.UpdateAsync(_imageId, request);

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
            var (isModified, _) = PrepareEquipmentImageUpdateRequest();

            if (isModified && !ShowConfirmationDialog())
            {
                return;
            }

            await NavigateBackAsync();
        }

        [RelayCommand]
        private void SelectFile()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "JPEG fájlok (*.jpg;*.jpeg)|*.jpg;*.jpeg",
                Title = "Kép kiválasztása",
                Multiselect = false
            };

            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult is not null && (bool)dialogResult)
            {
                MainImageUrl = new Uri(openFileDialog.FileName);
                DisplayName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
            }
        }

        private static async Task NavigateBackAsync()
        {
            await Navigator.Instance.NavigateToAsync<EquipmentImageListViewModel>();
        }

        private static bool ShowConfirmationDialog()
        {
            var result = MessageBox.Show("Biztosan kilép mentés nélkül?", "Nem mentett módosítások!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        private (bool isModified, UpdateEquipmentImageRequest request) PrepareEquipmentImageUpdateRequest()
        {
            var request = new UpdateEquipmentImageRequest();
            var isModified = false;

            if (DisplayName.Trim() != (_originalDisplayName ?? string.Empty))
            {
                request = request with { DisplayName = DisplayName.Trim() };
                isModified = true;
            }

            if (MainImageUrl != _originalImageUrl)
            {
                byte[] imageBytes = File.ReadAllBytes(MainImageUrl.LocalPath);
                string base64String = Convert.ToBase64String(imageBytes);

                request = request with { Base64ImageData = base64String };
                isModified = true;
            }

            return (isModified, request);
        }
    }
}
