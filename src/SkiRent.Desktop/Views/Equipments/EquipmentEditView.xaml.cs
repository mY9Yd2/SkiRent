using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.Equipments;

namespace SkiRent.Desktop.Views.Equipments
{
    /// <summary>
    /// Interaction logic for EquipmentEditView.xaml
    /// </summary>
    public partial class EquipmentEditView : UserControl
    {
        public EquipmentEditView()
        {
            InitializeComponent();
        }

        public EquipmentEditView(EquipmentEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
