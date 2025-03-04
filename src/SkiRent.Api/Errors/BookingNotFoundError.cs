using FluentResults;

namespace SkiRent.Api.Errors;

public class BookingNotFoundError : Error
{
    public BookingNotFoundError(int bookingId)
        : base($"Booking with id '{bookingId}' not found.")
    {
        Metadata.Add(nameof(bookingId), bookingId);
    }
}
