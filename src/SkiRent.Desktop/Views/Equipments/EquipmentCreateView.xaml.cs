using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.Equipments;

namespace SkiRent.Desktop.Views.Equipments
{
    /// <summary>
    /// Interaction logic for EquipmentCreateView.xaml
    /// </summary>
    public partial class EquipmentCreateView : UserControl
    {
        public EquipmentCreateView()
        {
            InitializeComponent();
        }

        public EquipmentCreateView(EquipmentCreateViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
