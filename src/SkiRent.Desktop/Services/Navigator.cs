namespace SkiRent.Desktop.Services
{
    public static class Navigator
    {
        public static INavigationService Instance { get; private set; } = null!;

        public static void Initialize(INavigationService navigationService)
        {
            Instance = navigationService;
        }
    }
}
