using System.IO.Abstractions;

using FluentResults;

using Microsoft.Extensions.Options;

using SkiRent.Api.Configurations;
using SkiRent.Api.Data.Auth;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;

namespace SkiRent.Api.Services.Invoices;

public class InvoiceService : IInvoiceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileSystem _fileSystem;
    private readonly AppSettings _appSettings;

    public InvoiceService(IUnitOfWork unitOfWork, IFileSystem fileSystem, IOptions<AppSettings> appSettings)
    {
        _unitOfWork = unitOfWork;
        _fileSystem = fileSystem;
        _appSettings = appSettings.Value;
    }

    public async Task<Result<byte[]>> GetAsync(Guid invoiceId, int userId, Func<string, bool> isInRole)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);

        if (invoice is null)
        {
            return Result.Fail(new InvoiceNotFoundError(invoiceId));
        }

        if (!isInRole(Roles.Admin) && invoice.UserId != userId)
        {
            return Result.Fail(new InvoiceAccessDeniedError(invoiceId));
        }

        var path = _fileSystem.Path.Combine(_appSettings.DataDirectoryPath, "Invoices", $"{invoiceId}.pdf");
        var result = await _fileSystem.File.ReadAllBytesAsync(path);

        return Result.Ok(result);
    }
}
