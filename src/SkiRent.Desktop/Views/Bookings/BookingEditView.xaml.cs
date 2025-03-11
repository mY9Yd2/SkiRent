using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.Bookings;

namespace SkiRent.Desktop.Views.Bookings
{
    /// <summary>
    /// Interaction logic for BookingEditView.xaml
    /// </summary>
    public partial class BookingEditView : UserControl
    {
        public BookingEditView()
        {
            InitializeComponent();
        }

        public BookingEditView(BookingEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
