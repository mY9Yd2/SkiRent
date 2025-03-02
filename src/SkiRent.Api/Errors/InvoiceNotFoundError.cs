using FluentResults;

namespace SkiRent.Api.Errors;

public class InvoiceNotFoundError : Error
{
    public InvoiceNotFoundError(Guid invoiceId)
        : base($"Invoice with id '{invoiceId}' not found.")
    {
        Metadata.Add(nameof(invoiceId), invoiceId);
    }
}
