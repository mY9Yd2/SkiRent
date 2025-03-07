using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.ViewModels.Base;

namespace SkiRent.Desktop.Services
{
    public interface INavigationService
    {
        public void NavigateTo<TBaseViewModel>() where TBaseViewModel : BaseViewModel;
        public void NavigateTo<TBaseViewModel>(Action<TBaseViewModel> initialize) where TBaseViewModel : BaseViewModel;
        public void SwitchTo<TBaseViewModel>() where TBaseViewModel : BaseViewModel, IViewUpdater;
    }
}
