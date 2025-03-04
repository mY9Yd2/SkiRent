using FluentResults;

namespace SkiRent.Api.Errors;

public class InvalidBookingStatusTransitionError : Error
{
    public InvalidBookingStatusTransitionError(string currentBookingStatus, string requestedBookingStatus)
        : base($"Cannot transition from {currentBookingStatus} to {requestedBookingStatus}.")
    {
        Metadata.Add(nameof(currentBookingStatus), currentBookingStatus);
        Metadata.Add(nameof(requestedBookingStatus), requestedBookingStatus);
    }
}
