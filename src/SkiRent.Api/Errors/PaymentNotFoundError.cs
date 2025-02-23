using FluentResults;

namespace SkiRent.Api.Errors;

public class PaymentNotFoundError : Error
{
    public PaymentNotFoundError(Guid paymentId)
        : base($"Payment with id '{paymentId}' not found.")
    {
        Metadata.Add(nameof(paymentId), paymentId);
    }
}
