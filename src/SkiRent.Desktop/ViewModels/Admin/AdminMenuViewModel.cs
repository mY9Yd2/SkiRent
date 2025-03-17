using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Desktop.ViewModels.Bookings;
using SkiRent.Desktop.ViewModels.EquipmentCategories;
using SkiRent.Desktop.ViewModels.EquipmentImages;
using SkiRent.Desktop.ViewModels.Equipments;
using SkiRent.Desktop.ViewModels.Invoices;
using SkiRent.Desktop.ViewModels.Users;

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

        [RelayCommand]
        private async Task ShowEquipmentCategoriesAsync()
        {
            await Navigator.Instance.NavigateToAsync<EquipmentCategoryListViewModel>();
        }

        [RelayCommand]
        private async Task ShowInvoicesAsync()
        {
            await Navigator.Instance.NavigateToAsync<InvoiceListViewModel>();
        }

        [RelayCommand]
        private async Task ShowUsersAsync()
        {
            await Navigator.Instance.NavigateToAsync<UserListViewModel>();
        }

        [RelayCommand]
        private async Task ShowEquipmentImagesAsync()
        {
            await Navigator.Instance.NavigateToAsync<EquipmentImageListViewModel>();
        }
    }
}
