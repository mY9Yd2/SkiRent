using System.Windows.Controls;

using SkiRent.Desktop.ViewModels.Invoices;

namespace SkiRent.Desktop.Views.Invoices
{
    /// <summary>
    /// Interaction logic for InvoiceListView.xaml
    /// </summary>
    public partial class InvoiceListView : UserControl
    {
        public InvoiceListView()
        {
            InitializeComponent();
        }

        public InvoiceListView(InvoiceListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
