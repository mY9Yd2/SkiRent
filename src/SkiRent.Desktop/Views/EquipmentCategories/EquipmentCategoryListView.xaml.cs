using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.EquipmentCategories;

namespace SkiRent.Desktop.Views.EquipmentCategories
{
    /// <summary>
    /// Interaction logic for EquipmentCategoryListView.xaml
    /// </summary>
    public partial class EquipmentCategoryListView : UserControl
    {
        public EquipmentCategoryListView()
        {
            InitializeComponent();
        }

        public EquipmentCategoryListView(EquipmentCategoryListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
