using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FluentValidation;

using Microsoft.AspNetCore.Http;
using Microsoft.Win32;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.Utils;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;

namespace SkiRent.Desktop.ViewModels.EquipmentImages
{
    public partial class EquipmentImageListViewModel : BaseViewModel, IInitializeAsync
    {
        private readonly ISkiRentApi _skiRentApi = null!;
        private readonly IValidator<IFormFile> _validator = null!;

        [ObservableProperty]
        private EquipmentImagesList _selectedEquipmentImage = null!;

        public ObservableCollection<EquipmentImagesList> EquipmentImages { get; } = [];

        public EquipmentImageListViewModel()
        { }

        public EquipmentImageListViewModel(ISkiRentApi skiRentApi, IValidator<IFormFile> validator)
        {
            _skiRentApi = skiRentApi;
            _validator = validator;
        }

        public async Task InitializeAsync()
        {
            var result = await _skiRentApi.EquipmentImages.GetAllAsync();

            if (result.IsSuccessful)
            {
                EquipmentImages.Clear();
                foreach (var image in result.Content)
                {
                    EquipmentImages.Add(new()
                    {
                        Id = image.Id,
                        DisplayName = image.DisplayName,
                        CreatedAt = image.CreatedAt,
                        ImageUrl = new Uri($"{_skiRentApi.Client.BaseAddress}images/{image.Id}.jpg?t={DateTimeOffset.UtcNow.Ticks}")
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
        private async Task SelectFilesAsync()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "JPEG fájlok (*.jpg;*.jpeg)|*.jpg;*.jpeg",
                Title = "Képek kiválasztása",
                Multiselect = true
            };

            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult is not null && (bool)dialogResult)
            {
                var failedFiles = new List<string>();

                foreach (var filePath in openFileDialog.FileNames)
                {
                    if (!await UploadFileAsync(filePath))
                    {
                        failedFiles.Add(Path.GetFileName(filePath));
                    }
                }

                if (failedFiles.Count > 0)
                {
                    var errorMessage = "A következő fájlok feltöltése sikertelen:\n- " + string.Join("\n- ", failedFiles);
                    MessageBox.Show(errorMessage, "Feltöltés sikertelen", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("A kép(ek) sikeresen feltöltve.", "Feltöltés sikeres", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                await RefreshAsync();
            }
        }

        [RelayCommand]
        private async Task ShowEquipmentImageEditCommand()
        {
            if (SelectedEquipmentImage is not null)
            {
                await Navigator.Instance.NavigateToAsync<EquipmentImageEditViewModel>(async vm =>
                    await vm.InitializeAsync(SelectedEquipmentImage.Id, SelectedEquipmentImage.DisplayName, SelectedEquipmentImage.ImageUrl));
            }
        }

        [RelayCommand]
        private async Task DeleteEquipmentImageAsync()
        {
            if (SelectedEquipmentImage is null)
            {
                return;
            }

            var result = MessageBox.Show("Biztosan törölni szeretné ezt a képet?", "Törlés megerősítése",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var deleteResult = await _skiRentApi.EquipmentImages.DeleteAsync(SelectedEquipmentImage.Id);

            if (deleteResult.IsSuccessful)
            {
                MessageBox.Show("A kép sikeresen törölve lett.", "Sikeres törlés",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                await RefreshAsync();
                return;
            }

            MessageBox.Show("A képet nem lehet törölni, mert még van hozzá kapcsolódó felszerelés.", "Hiba",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async Task<bool> UploadFileAsync(string filePath)
        {
            var fileName = Path.GetFileName(filePath);

            await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);

            var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "formFile", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = MediaTypeNames.Image.Jpeg
            };

            var validationResult = await _validator.ValidateAsync(formFile);

            if (!validationResult.IsValid)
            {
                return false;
            }

            var streamContent = new StreamContent(formFile.OpenReadStream())
            {
                Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(formFile.ContentType) }
            };

            var formData = new MultipartFormDataContent
            {
                { streamContent, "formFile", formFile.FileName }
            };

            var response = await _skiRentApi.EquipmentImages.CreateAsync(formData);

            return response.IsSuccessStatusCode;
        }
    }
}
