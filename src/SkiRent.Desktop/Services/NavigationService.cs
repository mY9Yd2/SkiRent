using Microsoft.Extensions.DependencyInjection;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Exceptions;
using SkiRent.Desktop.ViewModels.Admin;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Desktop.ViewModels.Bookings;
using SkiRent.Desktop.ViewModels.EquipmentCategories;
using SkiRent.Desktop.ViewModels.EquipmentImages;
using SkiRent.Desktop.ViewModels.Equipments;
using SkiRent.Desktop.ViewModels.Invoices;
using SkiRent.Desktop.ViewModels.Main;
using SkiRent.Desktop.ViewModels.Users;

namespace SkiRent.Desktop.Services
{
    /// <summary>
    /// Provides navigation functionality between different view models in the application.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private Action<BaseViewModel> _updateCurrentView = null!;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            UpdateAction<MainWindowViewModel>();
        }

        /// <summary>
        /// Navigates to the specified view model type and executes an asynchronous initialization function.
        /// </summary>
        /// <typeparam name="TBaseViewModel">The type of view model to navigate to.</typeparam>
        /// <param name="initialize">An asynchronous delegate to initialize the view model.</param>
        public async Task NavigateToAsync<TBaseViewModel>(Func<TBaseViewModel, Task> initialize) where TBaseViewModel : BaseViewModel
        {
            var viewModel = _serviceProvider.GetRequiredService<TBaseViewModel>();
            _updateCurrentView(viewModel);
            await initialize(viewModel);
            UpdateTitle(viewModel);
        }

        /// <summary>
        /// Navigates to the specified view model type and calls its default asynchronous initializer if implemented.
        /// </summary>
        /// <typeparam name="TBaseViewModel">The type of view model to navigate to.</typeparam>
        public async Task NavigateToAsync<TBaseViewModel>() where TBaseViewModel : BaseViewModel
        {
            var viewModel = _serviceProvider.GetRequiredService<TBaseViewModel>();
            _updateCurrentView(viewModel);
            if (viewModel is IInitializeAsync initialize)
            {
                await initialize.InitializeAsync();
            }
            UpdateTitle(viewModel);
        }

        /// <summary>
        /// Switches to a different main view model and updates the view updater delegate accordingly.
        /// </summary>
        /// <typeparam name="TBaseViewModel">The type of view model to switch to.</typeparam>
        /// <exception cref="UnhandledViewUpdaterException">Thrown if the view model type is not handled.</exception>
        public void SwitchTo<TBaseViewModel>() where TBaseViewModel : BaseViewModel, IViewUpdater
        {
            var viewModel = _serviceProvider.GetRequiredService<TBaseViewModel>();

            if (viewModel is MainWindowViewModel)
            {
                UpdateAction<MainWindowViewModel>();
            }
            else if (viewModel is AdminMainViewModel)
            {
                _updateCurrentView(viewModel);
                UpdateAction<AdminMainViewModel>();
            }
            else
            {
                throw new UnhandledViewUpdaterException($"Unhandled view updater type: {typeof(TBaseViewModel).Name}.");
            }
        }

        /// <summary>
        /// Updates the current view updater action based on the specified view model type.
        /// </summary>
        /// <typeparam name="TBaseViewModel">The type of the view model implementing <see cref="IViewUpdater"/>.</typeparam>
        private void UpdateAction<TBaseViewModel>() where TBaseViewModel : BaseViewModel, IViewUpdater
        {
            var viewModel = _serviceProvider.GetRequiredService<TBaseViewModel>();
            _updateCurrentView = viewModel.UpdateCurrentView;
        }

        /// <summary>
        /// Updates the window title based on the current view model type.
        /// </summary>
        /// <typeparam name="TBaseViewModel">The type of the view model.</typeparam>
        /// <param name="viewModel">The instance of the current view model.</param>
        private void UpdateTitle<TBaseViewModel>(TBaseViewModel viewModel) where TBaseViewModel : BaseViewModel
        {
            var window = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            string baseTitle = "SkiRent";
            window.Title = viewModel switch
            {
                EquipmentListViewModel => $"{baseTitle} - Felszerelések",
                EquipmentEditViewModel => $"{baseTitle} - Felszerelés szerkesztése",
                EquipmentCreateViewModel => $"{baseTitle} - Felszerelés létrehozása",
                BookingListViewModel => $"{baseTitle} - Foglalások",
                BookingEditViewModel => $"{baseTitle} - Foglalás szerkesztése",
                BookingItemListViewModel => $"{baseTitle} - Tétellista",
                EquipmentCategoryListViewModel => $"{baseTitle} - Felszerelés kategóriák",
                EquipmentCategoryEditViewModel => $"{baseTitle} - Felszerelés kategória szerkesztése",
                EquipmentCategoryCreateViewModel => $"{baseTitle} - Felszerelés kategória létrehozása",
                InvoiceListViewModel => $"{baseTitle} - Számlák",
                UserListViewModel => $"{baseTitle} - Felhasználók",
                UserEditViewModel => $"{baseTitle} - Felhasználó szerkesztése",
                UserCreateViewModel => $"{baseTitle} - Felhasználó létrehozása",
                EquipmentImageListViewModel => $"{baseTitle} - Felszerelésképek",
                EquipmentImageEditViewModel => $"{baseTitle} - Felszereléskép szerkesztése",
                _ => baseTitle
            };
        }
    }
}
