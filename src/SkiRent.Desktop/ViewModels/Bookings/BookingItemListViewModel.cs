using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.Utils;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Desktop.ViewModels.Bookings
{
    public partial class BookingItemListViewModel : BaseViewModel, IInitializeAsync<int, IEnumerable<BookingItemSummary>>
    {
        private int _bookingId;

        public ObservableCollection<BookingItemList> Items { get; } = [];

        public BookingItemListViewModel()
        { }

        public Task InitializeAsync(int bookingId, IEnumerable<BookingItemSummary> items)
        {
            _bookingId = bookingId;
            foreach (var item in items)
            {
                Items.Add(new()
                {
                    Name = item.Name,
                    Quantity = item.Quantity,
                    PricePerDay = CultureFormatHelper.FormatCurrency(item.PricePerDay),
                    TotalPrice = CultureFormatHelper.FormatCurrency(item.TotalPrice)
                });
            }
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task BackAsync()
        {
            await Navigator.Instance.NavigateToAsync<BookingEditViewModel>(async vm =>
                await vm.InitializeAsync(_bookingId));
        }
    }
}
