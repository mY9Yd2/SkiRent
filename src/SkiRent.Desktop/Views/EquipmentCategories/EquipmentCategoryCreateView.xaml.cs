using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.EquipmentCategories;

namespace SkiRent.Desktop.Views.EquipmentCategories
{
    /// <summary>
    /// Interaction logic for EquipmentCategoryCreateView.xaml
    /// </summary>
    public partial class EquipmentCategoryCreateView : UserControl
    {
        public EquipmentCategoryCreateView()
        {
            InitializeComponent();
        }

        public EquipmentCategoryCreateView(EquipmentCategoryCreateViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
