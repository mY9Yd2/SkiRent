using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;

namespace SkiRent.Desktop.ViewModels.Bookings
{
    public partial class BookingListViewModel : BaseViewModel, IInitializeAsync
    {
        private readonly ISkiRentApi _skiRentApi = null!;

        public BookingListViewModel()
        { }

        public BookingListViewModel(ISkiRentApi skiRentApi)
        {
            _skiRentApi = skiRentApi;
        }

        public ObservableCollection<BookingList> Bookings { get; } = [];

        [ObservableProperty]
        private BookingList _selectedBooking = null!;

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
                        Status = booking.Status,
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
            throw new NotImplementedException();
        }
    }
}
