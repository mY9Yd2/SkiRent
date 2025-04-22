using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Abstractions;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SkiRent.Desktop.Contracts;
using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Base;
using SkiRent.Shared.Clients;

namespace SkiRent.Desktop.ViewModels.Invoices
{
    public partial class InvoiceListViewModel : BaseViewModel, IInitializeAsync
    {
        private readonly ISkiRentApi _skiRentApi = null!;
        private readonly IFileSystem _fileSystem = null!;
        private readonly IProcessService _processService = null!;

        [ObservableProperty]
        private InvoiceList _selectedInvoice = null!;

        public ObservableCollection<InvoiceList> Invoices { get; } = [];

        public InvoiceListViewModel()
        { }

        public InvoiceListViewModel(ISkiRentApi skiRentApi, IFileSystem fileSystem, IProcessService processService)
        {
            _skiRentApi = skiRentApi;
            _fileSystem = fileSystem;
            _processService = processService;
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

        [RelayCommand]
        private async Task ShowPdf()
        {
            var result = await _skiRentApi.Invoices.GetAsync(SelectedInvoice.Id);

            if (result.IsSuccessful)
            {
                var invoiceFileName = result.ContentHeaders?.ContentDisposition?.FileNameStar
                    ?? $"Számla_{SelectedInvoice.Id}.pdf";

                var contentBytes = await result.Content.ReadAsByteArrayAsync();

                var tempPath = _fileSystem.Path.GetFullPath(_fileSystem.Path.GetTempPath());
                var path = _fileSystem.Path.Combine(tempPath, "SkiRent");

                var directory = _fileSystem.Directory.CreateDirectory(path);
                var filePath = _fileSystem.Path.Combine(directory.FullName, invoiceFileName);

                await _fileSystem.File.WriteAllBytesAsync(filePath, contentBytes);

                _processService.Start(new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                });
            }
        }
    }
}
