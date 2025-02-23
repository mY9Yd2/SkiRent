using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Shared.Contracts.Bookings
{
    public record CreateBookingRequest
    {
        public required IEnumerable<EquipmentBooking> Equipments { get; init; }

        public required DateOnly StartDate { get; init; }

        public required DateOnly EndDate { get; init; }

        public required Uri SuccessUrl { get; init; }

        public required Uri CancelUrl { get; init; }
    }

    public record CreatedBookingResponse
    {
        public required int Id { get; init; }

        public required Guid PaymentId { get; init; }

        public required Uri PaymentUrl { get; init; }
    }
}
