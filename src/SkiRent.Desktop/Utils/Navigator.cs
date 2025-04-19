using SkiRent.Desktop.Services;

namespace SkiRent.Desktop.Utils
{
    /// <summary>
    /// Provides global access to the application's navigation service.
    /// </summary>
    public static class Navigator
    {
        /// <summary>
        /// Gets the globally accessible instance of the <see cref="INavigationService"/>.
        /// </summary>
        public static INavigationService Instance { get; private set; } = null!;

        /// <summary>
        /// Initializes the <see cref="Navigator"/> with the specified <see cref="INavigationService"/> instance.
        /// This method should be called once during application startup.
        /// </summary>
        /// <param name="navigationService">The navigation service to assign as the global instance.</param>
        public static void Initialize(INavigationService navigationService)
        {
            Instance = navigationService;
        }
    }
}
