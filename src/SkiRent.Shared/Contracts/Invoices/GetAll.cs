namespace SkiRent.Shared.Contracts.Invoices
{
    public record GetAllInvoicesResponse
    {
        public required Guid Id { get; init; }

        public required int? UserId { get; init; }

        public required int? BookingId { get; init; }

        public required DateTimeOffset CreatedAt { get; init; }

        public required string? Email { get; init; }
    }
}
