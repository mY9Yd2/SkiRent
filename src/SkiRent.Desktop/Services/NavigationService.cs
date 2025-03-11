using Microsoft.Extensions.DependencyInjection;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Exceptions;
using SkiRent.Desktop.ViewModels.Admin;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Desktop.ViewModels.Equipments;
using SkiRent.Desktop.ViewModels.Main;

namespace SkiRent.Desktop.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private Action<BaseViewModel> _updateCurrentView = null!;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            UpdateAction<MainWindowViewModel>();
        }

        public async Task NavigateToAsync<TBaseViewModel>(Func<TBaseViewModel, Task> initialize) where TBaseViewModel : BaseViewModel
        {
            var viewModel = _serviceProvider.GetRequiredService<TBaseViewModel>();
            _updateCurrentView(viewModel);
            await initialize(viewModel);
            UpdateTitle(viewModel);
        }

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

        private void UpdateAction<TBaseViewModel>() where TBaseViewModel : BaseViewModel, IViewUpdater
        {
            var viewModel = _serviceProvider.GetRequiredService<TBaseViewModel>();
            _updateCurrentView = viewModel.UpdateCurrentView;
        }

        private void UpdateTitle<TBaseViewModel>(TBaseViewModel viewModel) where TBaseViewModel : BaseViewModel
        {
            var window = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            string baseTitle = "SkiRent";
            window.Title = viewModel switch
            {
                EquipmentListViewModel => $"{baseTitle} - Felszerelések",
                _ => baseTitle
            };
        }
    }
}
