using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Contracts.Bookings
{
    public record GetBookingResponse
    {
        public required int Id { get; init; }

        public required int UserId { get; init; }

        public required DateOnly StartDate { get; init; }

        public required DateOnly EndDate { get; init; }

        public required decimal TotalPrice { get; init; }

        public required Guid PaymentId { get; init; }

        public required BookingStatusTypes Status { get; init; }

        public required DateTimeOffset CreatedAt { get; init; }

        public required DateTimeOffset UpdatedAt { get; init; }

        public required IEnumerable<BookingItemSummary> Items { get; init; }

        public required int RentalDays { get; init; }

        public required bool IsOverdue { get; init; }

        public required PersonalDetails PersonalDetails { get; init; }
    }
}
