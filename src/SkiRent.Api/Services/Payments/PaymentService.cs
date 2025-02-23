using FluentResults;

using SkiRent.Api.Data.Models;
using SkiRent.Api.Data.UnitOfWork;
using SkiRent.Api.Errors;
using SkiRent.Shared.Contracts.Payments;

namespace SkiRent.Api.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ProcessPaymentCallbackAsync(PaymentResult paymentResult)
    {
        var booking = await _unitOfWork.Bookings.GetBookingWithItemsAsync(paymentResult.PaymentId);

        if (booking is null)
        {
            return Result.Fail(new PaymentNotFoundError(paymentResult.PaymentId));
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

        return Result.Ok();
    }
}
