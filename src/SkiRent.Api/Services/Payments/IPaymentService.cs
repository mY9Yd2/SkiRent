using FluentResults;

using SkiRent.Shared.Contracts.Payments;

namespace SkiRent.Api.Services.Payments;

public interface IPaymentService
{
    public Task<Result> ProcessPaymentCallbackAsync(PaymentResult paymentResult);
}
