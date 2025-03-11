using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.Bookings;

namespace SkiRent.Desktop.Views.Bookings
{
    /// <summary>
    /// Interaction logic for BookingItemListView.xaml
    /// </summary>
    public partial class BookingItemListView : UserControl
    {
        public BookingItemListView()
        {
            InitializeComponent();
        }

        public BookingItemListView(BookingItemListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
