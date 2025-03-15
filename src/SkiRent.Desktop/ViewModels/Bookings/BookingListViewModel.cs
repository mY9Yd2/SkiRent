using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.Utils;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;

namespace SkiRent.Desktop.ViewModels.Bookings
{
    public partial class BookingListViewModel : BaseViewModel, IInitializeAsync
    {
        private readonly ISkiRentApi _skiRentApi = null!;

        [ObservableProperty]
        private BookingList _selectedBooking = null!;

        public ObservableCollection<BookingList> Bookings { get; } = [];

        public BookingListViewModel()
        { }

        public BookingListViewModel(ISkiRentApi skiRentApi)
        {
            _skiRentApi = skiRentApi;
        }

        public async Task InitializeAsync()
        {
            var result = await _skiRentApi.Bookings.GetAllAsync();

            if (result.IsSuccessful)
            {
                Bookings.Clear();
                foreach (var booking in result.Content)
                {
                    Bookings.Add(new()
                    {
                        Id = booking.Id,
                        StartDate = booking.StartDate,
                        EndDate = booking.EndDate,
                        TotalPrice = booking.TotalPrice,
                        PaymentId = booking.PaymentId,
                        Status = BookingStatusHelper.GetLocalizedString(booking.Status),
                        CreatedAt = booking.CreatedAt,
                        IsOverdue = booking.IsOverdue
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
        private async Task ShowBookingEditAsync()
        {
            if (SelectedBooking is not null)
            {
                await Navigator.Instance.NavigateToAsync<BookingEditViewModel>(async vm =>
                    await vm.InitializeAsync(SelectedBooking.Id));
            }
        }

        [RelayCommand]
        private async Task DeleteBookingAsync()
        {
            if (SelectedBooking is not null)
            {
                var result = MessageBox.Show("Biztosan törölni szeretné ezt a foglalást?", "Törlés megerősítése",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                var deleteResult = await _skiRentApi.Bookings.DeleteAsync(SelectedBooking.Id);

                if (deleteResult.IsSuccessful)
                {
                    MessageBox.Show("A foglalás sikeresen törölve lett.", "Sikeres törlés",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    await RefreshAsync();
                }
                else
                {
                    MessageBox.Show("A foglalást nem lehet törölni, mert még nincs 'Törölve' vagy 'Visszahozva' állapotban.", "Hiba",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
