namespace SkiRent.Desktop.Models
{
    public record BookingList
    {
        public required int Id { get; init; }

        public required DateOnly StartDate { get; init; }

        public required DateOnly EndDate { get; init; }

        public required string TotalPrice { get; init; }

        public required Guid PaymentId { get; init; }

        public required string Status { get; init; }

        public required DateTimeOffset CreatedAt { get; init; }

        public required bool IsOverdue { get; init; }
    }
}
