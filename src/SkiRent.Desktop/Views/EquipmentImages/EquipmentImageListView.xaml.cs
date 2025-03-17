using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.EquipmentImages;

namespace SkiRent.Desktop.Views.EquipmentImages
{
    /// <summary>
    /// Interaction logic for EquipmentImageListView.xaml
    /// </summary>
    public partial class EquipmentImageListView : UserControl
    {
        public EquipmentImageListView()
        {
            InitializeComponent();
        }

        public EquipmentImageListView(EquipmentImageListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
