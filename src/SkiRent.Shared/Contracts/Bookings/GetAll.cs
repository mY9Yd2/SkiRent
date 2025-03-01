using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Contracts.Bookings
{
    public record GetAllBookingResponse
    {
        public int Id { get; init; }

        public DateOnly StartDate { get; init; }

        public DateOnly EndDate { get; init; }

        public decimal TotalPrice { get; init; }

        public Guid PaymentId { get; init; }

        public BookingStatusTypes Status { get; init; }
    }
}
