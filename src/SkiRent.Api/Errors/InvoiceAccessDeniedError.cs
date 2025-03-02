using FluentResults;

namespace SkiRent.Api.Errors;

public class InvoiceAccessDeniedError : Error
{
    public InvoiceAccessDeniedError(Guid invoiceId)
        : base($"Access to the invoice with id {invoiceId} is denied.")
    {
        Metadata.Add(nameof(invoiceId), invoiceId);
    }
}
