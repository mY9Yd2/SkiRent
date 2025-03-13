using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;

namespace SkiRent.Desktop.ViewModels.Invoices
{
    public partial class InvoiceListViewModel : BaseViewModel, IInitializeAsync
    {
        private readonly ISkiRentApi _skiRentApi = null!;

        [ObservableProperty]
        private InvoiceList _selectedInvoice = null!;

        public ObservableCollection<InvoiceList> Invoices { get; } = [];

        public InvoiceListViewModel()
        { }

        public InvoiceListViewModel(ISkiRentApi skiRentApi)
        {
            _skiRentApi = skiRentApi;
        }

        public async Task InitializeAsync()
        {
            var result = await _skiRentApi.Invoices.GetAllAsync();

            if (result.IsSuccessful)
            {
                Invoices.Clear();
                foreach (var invoice in result.Content)
                {
                    Invoices.Add(new()
                    {
                        Id = invoice.Id,
                        UserId = invoice.UserId,
                        BookingId = invoice.BookingId,
                        CreatedAt = invoice.CreatedAt,
                        Email = invoice.Email
                    });
                }
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            await InitializeAsync();
        }
    }
}
