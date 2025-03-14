using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.Users;

namespace SkiRent.Desktop.Views.Users
{
    /// <summary>
    /// Interaction logic for UserEditView.xaml
    /// </summary>
    public partial class UserEditView : UserControl
    {
        public UserEditView()
        {
            InitializeComponent();
        }

        public UserEditView(UserEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
