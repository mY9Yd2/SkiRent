using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.ViewModels.Base;

namespace SkiRent.Desktop.Services
{
    public interface INavigationService
    {
        public Task NavigateToAsync<TBaseViewModel>() where TBaseViewModel : BaseViewModel;
        public Task NavigateToAsync<TBaseViewModel>(Func<TBaseViewModel, Task> initialize) where TBaseViewModel : BaseViewModel;
        public void SwitchTo<TBaseViewModel>() where TBaseViewModel : BaseViewModel, IViewUpdater;
    }
}
