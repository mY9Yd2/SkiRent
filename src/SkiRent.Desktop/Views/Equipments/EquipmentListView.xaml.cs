using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.Equipments;

namespace SkiRent.Desktop.Views.Equipments
{
    /// <summary>
    /// Interaction logic for EquipmentView.xaml
    /// </summary>
    public partial class EquipmentListView : UserControl
    {
        public EquipmentListView()
        {
            InitializeComponent();
        }

        public EquipmentListView(EquipmentViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
