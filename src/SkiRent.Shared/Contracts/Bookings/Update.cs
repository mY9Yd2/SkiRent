using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Contracts.Bookings
{
    public record UpdateBookingRequest
    {
        public required BookingStatusTypes Status { get; init; }
    }
}
