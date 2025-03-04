using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Contracts.Bookings
{
    public record GetAllBookingResponse
    {
        public required int Id { get; init; }

        public required DateOnly StartDate { get; init; }

        public required DateOnly EndDate { get; init; }

        public required decimal TotalPrice { get; init; }

        public required Guid PaymentId { get; init; }

        public required BookingStatusTypes Status { get; init; }

        public required DateTimeOffset CreatedAt { get; init; }
    }
}
