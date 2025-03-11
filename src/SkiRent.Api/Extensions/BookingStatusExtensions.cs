using SkiRent.Api.Data.Models;
using SkiRent.Api.Exceptions;
using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Api.Extensions;

public static class BookingStatusExtensions
{
    public static string ToBookingStatusString(this BookingStatusTypes bookingStatus)
    {
        return bookingStatus switch
        {
            BookingStatusTypes.Pending => BookingStatus.Pending,
            BookingStatusTypes.Paid => BookingStatus.Paid,
            BookingStatusTypes.InDelivery => BookingStatus.InDelivery,
            BookingStatusTypes.Received => BookingStatus.Received,
            BookingStatusTypes.Cancelled => BookingStatus.Cancelled,
            BookingStatusTypes.Returned => BookingStatus.Returned,
            _ => throw new UnhandledBookingStatusException($"Unhandled booking status type: {bookingStatus}.")
        };
    }
}
