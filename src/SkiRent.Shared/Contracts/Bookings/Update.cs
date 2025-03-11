using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Contracts.Bookings
{
    public record UpdateBookingRequest
    {
        public BookingStatusTypes? Status { get; init; }
    }
}
