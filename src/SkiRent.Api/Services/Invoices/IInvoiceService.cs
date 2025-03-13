using FluentResults;

using SkiRent.Shared.Contracts.Invoices;

namespace SkiRent.Api.Services.Invoices;

public interface IInvoiceService
{
    public Task<Result<byte[]>> GetAsync(Guid invoiceId, int userId, Func<string, bool> isInRole);
    public Task<Result<IEnumerable<GetAllInvoicesResponse>>> GetAllAsync();
}
