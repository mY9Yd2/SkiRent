using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Desktop.ViewModels.Bookings;
using SkiRent.Desktop.ViewModels.Equipments;

namespace SkiRent.Desktop.ViewModels.Admin
{
    public partial class AdminMenuViewModel : BaseViewModel
    {
        public AdminMenuViewModel()
        { }

        [RelayCommand]
        private async Task ShowEquipmentsAsync()
        {
            await Navigator.Instance.NavigateToAsync<EquipmentListViewModel>();
        }

        [RelayCommand]
        private async Task ShowBookingsAsync()
        {
            await Navigator.Instance.NavigateToAsync<BookingListViewModel>();
        }
    }
}
