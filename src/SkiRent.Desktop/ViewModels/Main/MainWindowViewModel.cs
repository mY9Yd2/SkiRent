using CommunityToolkit.Mvvm.ComponentModel;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.ViewModels.Base;

namespace SkiRent.Desktop.ViewModels.Main
{
    public partial class MainWindowViewModel : BaseViewModel, IViewUpdater
    {
        [ObservableProperty]
        private BaseViewModel _currentView = null!;

        [ObservableProperty]
        private string _title = string.Empty;

        public void UpdateCurrentView(BaseViewModel viewModel)
        {
            if (CurrentView is null || CurrentView.GetType().Name != viewModel.GetType().Name)
            {
                CurrentView = viewModel;
            }
        }
    }
}
