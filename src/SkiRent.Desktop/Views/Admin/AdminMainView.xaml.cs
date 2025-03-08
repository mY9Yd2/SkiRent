using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.Admin;

namespace SkiRent.Desktop.Views.Admin
{
    /// <summary>
    /// Interaction logic for AdminMainView.xaml
    /// </summary>
    public partial class AdminMainView : UserControl
    {
        public AdminMainView()
        {
            InitializeComponent();
        }

        public AdminMainView(AdminMainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
