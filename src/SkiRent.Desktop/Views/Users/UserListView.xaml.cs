using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.Users;

namespace SkiRent.Desktop.Views.Users
{
    /// <summary>
    /// Interaction logic for UserListView.xaml
    /// </summary>
    public partial class UserListView : UserControl
    {
        public UserListView()
        {
            InitializeComponent();
        }

        public UserListView(UserListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
