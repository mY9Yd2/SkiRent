using System.Windows;
using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.Main;

namespace SkiRent.Desktop.Views.Main
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        public MainView(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox
                && DataContext is MainViewModel viewModel)
            {
                viewModel.Password = passwordBox.Password;
            }
        }
    }
}
