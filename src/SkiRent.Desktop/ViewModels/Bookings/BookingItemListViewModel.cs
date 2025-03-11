using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Desktop.ViewModels.Bookings
{
    public partial class BookingItemListViewModel : BaseViewModel, IInitializeAsync<int, IEnumerable<BookingItemSummary>>
    {
        public BookingItemListViewModel()
        { }

        public Task InitializeAsync(int bookingId, IEnumerable<BookingItemSummary> items)
        {
            _bookingId = bookingId;
            foreach (var item in items)
            {
                Items.Add(item);
            }
            return Task.CompletedTask;
        }

        private int _bookingId;

        public ObservableCollection<BookingItemSummary> Items { get; } = [];

        [RelayCommand]
        private async Task BackAsync()
        {
            await Navigator.Instance.NavigateToAsync<BookingEditViewModel>(async vm =>
                await vm.InitializeAsync(_bookingId));
        }
    }
}
