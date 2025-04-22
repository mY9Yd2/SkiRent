using System.Diagnostics;
using System.IO.Abstractions.TestingHelpers;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

using AutoFixture;

using NSubstitute;

using Refit;

using SkiRent.Desktop.Models;
using SkiRent.Desktop.Services;
using SkiRent.Desktop.ViewModels.Invoices;
using SkiRent.Shared.Clients;
using SkiRent.Shared.Contracts.Invoices;

namespace SkiRent.UnitTests.Systems.Desktop.ViewModels.Invoices;

public class TestInvoiceListViewModel
{
    private Fixture _fixture;
    private ISkiRentApi _skiRentApi;
    private IInvoicesApi _invoicesApi;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();

        _skiRentApi = Substitute.For<ISkiRentApi>();
        _invoicesApi = Substitute.For<IInvoicesApi>();
        _skiRentApi.Invoices.Returns(_invoicesApi);
    }

    [Test]
    public async Task InitializeAsync_PopulatesInvoices_WhenApiCallIsSuccessful()
    {
        // Arrange
        var invoiceDtos = _fixture.CreateMany<GetAllInvoiceResponse>(3);
        var response = new ApiResponse<IEnumerable<GetAllInvoiceResponse>>(new(HttpStatusCode.OK), invoiceDtos, null!);

        _invoicesApi.GetAllAsync()
            .Returns(response);

        var viewModel = new InvoiceListViewModel(_skiRentApi, null!, null!);

        viewModel.Invoices.Add(_fixture.Create<InvoiceList>());

        var expectedInvoices = invoiceDtos.Select(dto => new InvoiceList
        {
            Id = dto.Id,
            UserId = dto.UserId,
            BookingId = dto.BookingId,
            CreatedAt = dto.CreatedAt,
            Email = dto.Email
        });

        // Act
        await viewModel.InitializeAsync();

        // Assert
        Assert.That(viewModel.Invoices, Has.Count.EqualTo(invoiceDtos.Count()));
        Assert.That(expectedInvoices.All(viewModel.Invoices.Contains));
    }

    [Test]
    public async Task RefreshCommand_CallsInitializeAsync()
    {
        // Arrange
        var invoiceDtos = _fixture.CreateMany<GetAllInvoiceResponse>(2);
        var response = new ApiResponse<IEnumerable<GetAllInvoiceResponse>>(new(HttpStatusCode.OK), invoiceDtos, null!);

        _invoicesApi.GetAllAsync()
            .Returns(response);

        var viewModel = new InvoiceListViewModel(_skiRentApi, null!, null!);

        // Act
        await viewModel.RefreshCommand.ExecuteAsync(null);

        // Assert
        Assert.That(viewModel.Invoices, Has.Count.EqualTo(invoiceDtos.Count()));
    }

    [Test]
    public async Task ShowPdf_OpensFile_WhenApiCallIsSuccessful()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();

        var processService = Substitute.For<IProcessService>();
        var invoice = _fixture.Create<InvoiceList>();
        var content = new ByteArrayContent(Encoding.UTF8.GetBytes("dummy-pdf-content"));

        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
            FileNameStar = "custom_invoice.pdf"
        };

        var response = new ApiResponse<HttpContent>(new(HttpStatusCode.OK), content, null!);

        _invoicesApi.GetAsync(invoice.Id)
            .Returns(response);

        var viewModel = new InvoiceListViewModel(_skiRentApi, mockFileSystem, processService)
        {
            SelectedInvoice = invoice
        };

        // Act
        await viewModel.ShowPdfCommand.ExecuteAsync(null);

        // Assert
        processService.Received(1).Start(Arg.Any<ProcessStartInfo>());
    }

    [Test]
    public void ShowPdf_DoesNothing_WhenApiCallFails()
    {
        // Arrange
        var invoice = _fixture.Create<InvoiceList>();
        var response = new ApiResponse<HttpContent>(new(HttpStatusCode.BadRequest), null, null!);

        _invoicesApi.GetAsync(invoice.Id)
            .Returns(response);

        var viewModel = new InvoiceListViewModel(_skiRentApi, null!, null!)
        {
            SelectedInvoice = invoice
        };

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await viewModel.ShowPdfCommand.ExecuteAsync(null));
    }
}
