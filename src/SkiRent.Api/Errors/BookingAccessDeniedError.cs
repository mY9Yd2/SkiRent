using FluentResults;

namespace SkiRent.Api.Errors;

public class BookingAccessDeniedError : Error
{
    public BookingAccessDeniedError(int bookingId)
        : base($"Access to the booking with id {bookingId} is denied.")
    {
        Metadata.Add(nameof(bookingId), bookingId);
    }
}
