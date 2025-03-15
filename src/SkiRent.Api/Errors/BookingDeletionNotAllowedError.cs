using FluentResults;

namespace SkiRent.Api.Errors;

public class BookingDeletionNotAllowedError : Error
{
    public BookingDeletionNotAllowedError(int bookingId)
        : base($"Booking with id '{bookingId}' cannot be deleted because it is neither Cancelled nor Returned.")
    {
        Metadata.Add(nameof(bookingId), bookingId);
    }
}
