using System.IO.Abstractions;

using FluentResults;

using Microsoft.Extensions.Options;

using QuestPDF.Fluent;
using QuestPDF.Helpers;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Exceptions;
using SkiRent.Shared.Contracts.Invoices;
using SkiRent.Shared.Contracts.Payments;

using ZiggyCreatures.Caching.Fusion;

namespace SkiRent.Api.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFusionCache _cache;
    private readonly AppSettings _appSettings;
    private readonly IFileSystem _fileSystem;

    public PaymentService(
        IUnitOfWork unitOfWork,
        [FromKeyedServices("SkiRent.Api.Cache")] IFusionCache cache,
        IOptions<AppSettings> appSettings,
        IFileSystem fileSystem)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
        _appSettings = appSettings.Value;
        _fileSystem = fileSystem;
    }

    public async Task<Result> ProcessPaymentCallbackAsync(PaymentResult paymentResult)
    {
        var booking = await _unitOfWork.Bookings.GetBookingWithItemsAsync(paymentResult.PaymentId);

        if (booking is null)
        {
            throw new PaymentNotFoundException($"No booking found for Payment ID: {paymentResult.PaymentId}");
        }

        booking.Status = paymentResult.IsSuccessful
            ? BookingStatus.Paid
            : BookingStatus.Cancelled;

        if (!paymentResult.IsSuccessful)
        {
            foreach (var item in booking.BookingItems)
            {
                item.Equipment.AvailableQuantity += item.Quantity;
            }
        }

        await _unitOfWork.SaveChangesAsync();

        if (paymentResult.IsSuccessful)
        {
            var fileName = await CreateInvoiceAsync(paymentResult.PaymentId, paymentResult.PaidAt);

            var invoice = new Invoice
            {
                UserId = booking.UserId,
                BookingId = booking.Id,
                FileName = fileName
            };

            await _unitOfWork.Invoices.AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();
        }

        return Result.Ok();
    }

    private async Task<string> CreateInvoiceAsync(Guid paymentId, DateTimeOffset? paidAt)
    {
        var invoiceRequest = await _cache.GetOrDefaultAsync<CreateInvoiceRequest>(paymentId.ToString());

        if (invoiceRequest is null)
        {
            throw new CreateInvoiceRequestNotFoundException($"Invoice with payment id '{paymentId}' not found.");
        }

        byte[] pdfFile = GenerateInvoicePdf(invoiceRequest, paidAt ?? TimeProvider.System.GetUtcNow());
        string fileName = await SaveInvoiceToFileAsync(invoiceRequest.PaymentId, pdfFile);

        await _cache.RemoveAsync(paymentId.ToString());

        return fileName;
    }

    private async Task<string> SaveInvoiceToFileAsync(Guid paymentId, byte[] pdfFile)
    {
        var path = _fileSystem.Path.Combine(_appSettings.DataDirectoryPath, "Invoices");
        var directory = _fileSystem.Directory.CreateDirectory(path);
        var fileName = $"{paymentId}.pdf";
        var filePath = _fileSystem.Path.Combine(directory.FullName, fileName);

        await _fileSystem.File.WriteAllBytesAsync(filePath, pdfFile);

        return fileName;
    }

    private static byte[] GenerateInvoicePdf(CreateInvoiceRequest request, DateTimeOffset paidAt)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Header()
                    .Text("Számla")
                    .AlignCenter()
                    .SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);

                page.Content()
                    .Column(col =>
                    {
                        col.Item().Height(10);
                        col.Item()
                            .Column(innerCol =>
                            {
                                innerCol.Item().Text("Számlaazonosító:").Bold();
                                innerCol.Item().Text(request.PaymentId.ToString());
                            });

                        col.Item().Height(10);
                        col.Item().Text($"Dátum: {paidAt.ToString("g", request.Culture)}");


                        col.Item().Height(10);
                        col.Item().Text("Kereskedő adatai:").Bold();
                        col.Item().Height(10);
                        col.Item().Text($"Név: {request.MerchantName}");

                        col.Item().Height(10);
                        col.Item().Text("Ügyfél adatai:").Bold();
                        col.Item().Height(10);
                        col.Item().Text($"Név: {request.PersonalDetails.FullName}");
                        col.Item().Text($"Cím: {request.PersonalDetails.Address.StreetAddress}, {request.PersonalDetails.Address.City}, {request.PersonalDetails.Address.PostalCode}, {request.PersonalDetails.Address.Country}");
                        col.Item().Height(10);
                        col.Item().Text($"Telefonszám: {request.PersonalDetails.PhoneNumber}");
                        col.Item().Text($"Mobiltelefonszám: {request.PersonalDetails.MobilePhoneNumber}");

                        col.Item().Height(10);
                        col.Item().Text($"Bérlési idő: {request.StartDate.ToString("o", request.Culture)} - {request.EndDate.ToString("o", request.Culture)}");
                        col.Item().Height(10);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);
                                columns.ConstantColumn(250);
                                columns.RelativeColumn();
                                columns.ConstantColumn(100);
                            });

                            table.Header(header =>
                            {
                                header.Cell().PaddingVertical(8).Text("#");
                                header.Cell().PaddingVertical(8).Text("Terméknév").Bold();
                                header.Cell().PaddingVertical(8).Text("Részlet").Bold();
                                header.Cell().PaddingVertical(8).Text("Ár").Bold();
                            });

                            for (int i = 0; i < request.Items.Count(); i++)
                            {
                                var item = request.Items.ElementAt(i);

                                table.Cell().PaddingVertical(8).Text($"{i + 1}");
                                table.Cell().PaddingVertical(8).Text(item.Name);
                                table.Cell().PaddingVertical(8).Text(item.SubText);
                                table.Cell().PaddingVertical(8).Text(item.TotalPrice.ToString("C0", request.Culture));
                            }
                        });

                        col.Item().Height(10);
                        col.Item().AlignRight().Text($"Teljes ár: {request.TotalPrice.ToString("C0", request.Culture)}").Bold().FontSize(14);
                    });
            });
        }).GeneratePdf();
    }
}
