using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.Admin;

namespace SkiRent.Desktop.Views.Admin
{
    /// <summary>
    /// Interaction logic for AdminMenu.xaml
    /// </summary>
    public partial class AdminMenu : UserControl
    {
        public AdminMenu()
        {
            InitializeComponent();
        }

        public AdminMenu(AdminMenuViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
