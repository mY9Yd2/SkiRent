using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.EquipmentImages;

namespace SkiRent.Desktop.Views.EquipmentImages
{
    /// <summary>
    /// Interaction logic for EquipmentImageEditView.xaml
    /// </summary>
    public partial class EquipmentImageEditView : UserControl
    {
        public EquipmentImageEditView()
        {
            InitializeComponent();
        }

        public EquipmentImageEditView(EquipmentImageEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
