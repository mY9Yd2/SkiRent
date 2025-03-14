using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.Users;

namespace SkiRent.Desktop.Views.Users
{
    /// <summary>
    /// Interaction logic for UserCreateView.xaml
    /// </summary>
    public partial class UserCreateView : UserControl
    {
        public UserCreateView()
        {
            InitializeComponent();
        }

        public UserCreateView(UserCreateViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
