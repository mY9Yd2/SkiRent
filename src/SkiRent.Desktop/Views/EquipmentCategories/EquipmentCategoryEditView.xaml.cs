using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.EquipmentCategories;

namespace SkiRent.Desktop.Views.EquipmentCategories
{
    /// <summary>
    /// Interaction logic for EquipmentCategoryEditView.xaml
    /// </summary>
    public partial class EquipmentCategoryEditView : UserControl
    {
        public EquipmentCategoryEditView()
        {
            InitializeComponent();
        }

        public EquipmentCategoryEditView(EquipmentCategoryEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
