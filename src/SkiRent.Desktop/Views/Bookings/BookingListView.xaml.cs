using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.Bookings;

namespace SkiRent.Desktop.Views.Bookings
{
    /// <summary>
    /// Interaction logic for BookingListView.xaml
    /// </summary>
    public partial class BookingListView : UserControl
    {
        public BookingListView()
        {
            InitializeComponent();
        }

        public BookingListView(BookingListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
