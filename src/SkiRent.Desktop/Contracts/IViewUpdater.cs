using SkiRent.Desktop.ViewModels.Base;

namespace SkiRent.Desktop.Contracts
{
    public interface IViewUpdater
    {
        public void UpdateCurrentView(BaseViewModel viewModel);
    }
}
