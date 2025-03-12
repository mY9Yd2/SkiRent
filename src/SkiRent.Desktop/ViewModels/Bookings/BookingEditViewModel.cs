using System.Globalization;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FluentValidation;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.Utils;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Bookings;

namespace SkiRent.Desktop.ViewModels.Bookings
{
    public partial class BookingEditViewModel : BaseViewModel, IInitializeAsync<int>
    {
        private readonly ISkiRentApi _skiRentApi = null!;

        public BookingEditViewModel()
        { }

        public BookingEditViewModel(ISkiRentApi skiRentApi, IValidator<UpdateBookingRequest> validator)
        {
            _skiRentApi = skiRentApi;
            _validator = validator;
        }

        public async Task InitializeAsync(int bookingId)
        {
            var result = await _skiRentApi.Bookings.GetAsync(bookingId);

            if (result.IsSuccessful)
            {
                SelectedBookingStatus = BookingStatusHelper.GetLocalizedString(result.Content.Status);
                OriginalBooking = result.Content;
                OriginalTotalPriceFormatted = OriginalBooking.TotalPrice.ToString("C0", CultureInfo.CreateSpecificCulture("hu-HU"));
            }
        }

        [ObservableProperty]
        private GetBookingResponse _originalBooking = null!;

        [ObservableProperty]
        private string _originalTotalPriceFormatted = null!;

        private readonly IValidator<UpdateBookingRequest> _validator = null!;

        [ObservableProperty]
        private string _selectedBookingStatus = string.Empty;

        public IEnumerable<string> BookingStatuses { get; } = BookingStatusHelper.GetAllLocalizedStatuses();

        [RelayCommand]
        private async Task SaveAsync()
        {
            var (isModified, request) = PrepareBookingUpdateRequest();

            if (!isModified)
            {
                await NavigateBackAsync();
                return;
            }

            await _validator.ValidateAndThrowAsync(request);

            var result = await _skiRentApi.Bookings.UpdateAsync(OriginalBooking.Id, request);

            if (result.IsSuccessful)
            {
                MessageBox.Show("A változtatások sikeresen elmentésre kerültek.", "Sikeres mentés", MessageBoxButton.OK, MessageBoxImage.Information);
                await NavigateBackAsync();
                return;
            }

            MessageBox.Show("Hiba történt a mentés során.", "Sikertelen mentés", MessageBoxButton.OK, MessageBoxImage.Error);
            await InitializeAsync(OriginalBooking.Id);
        }

        [RelayCommand]
        private async Task ShowItemsAsync()
        {
            await Navigator.Instance.NavigateToAsync<BookingItemListViewModel>(async vm =>
                await vm.InitializeAsync(OriginalBooking.Id, OriginalBooking.Items));
        }

        [RelayCommand]
        private async Task BackAsync()
        {
            var (isModified, _) = PrepareBookingUpdateRequest();

            if (isModified && !ShowConfirmationDialog())
            {
                return;
            }

            await NavigateBackAsync();
        }

        private static async Task NavigateBackAsync()
        {
            await Navigator.Instance.NavigateToAsync<BookingListViewModel>();
        }

        private static bool ShowConfirmationDialog()
        {
            var result = MessageBox.Show("Biztosan kilép mentés nélkül?", "Nem mentett módosítások!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        private (bool isModified, UpdateBookingRequest request) PrepareBookingUpdateRequest()
        {
            var request = new UpdateBookingRequest();
            var isModified = false;

            var status = BookingStatusHelper.GetStatusFromLocalizedString(SelectedBookingStatus);

            if (status != OriginalBooking.Status)
            {
                request = request with { Status = status };
                isModified = true;
            }

            return (isModified, request);
        }
    }
}
