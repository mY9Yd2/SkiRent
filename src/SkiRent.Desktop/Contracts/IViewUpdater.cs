using SkiRent.Desktop.ViewModels.Base;

namespace SkiRent.Desktop.Contracts
{
    /// <summary>
    /// Defines a contract for view models that can update the currently displayed view.
    /// </summary>
    public interface IViewUpdater
    {
        /// <summary>
        /// Updates the current view to display the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model to display.</param>
        public void UpdateCurrentView(BaseViewModel viewModel);
    }
}
